namespace RPNCalcLib

open System
open System.Collections.Generic
open System.Diagnostics

module Utils =
  let fixFuncName (name: string) =
    let fname = name.Substring(1)
    if name.StartsWith("L") then "<"+fname+">" else "'"+fname+"'"

type CalcSyntaxError() =
  inherit Exception("Syntax error")

type CalcStackError(underflow: bool) =
  inherit Exception(sprintf "Stack %sflow" (if underflow then "under" else "over"))
  member this.IsUnderflow = underflow

type CalcFunctionError(name: string) =
  inherit Exception(sprintf "No func %s" (Utils.fixFuncName name))

type CalcMaxIterationError() =
  inherit Exception("Iteration limit")

type CalcMaxRecursionError() =
  inherit Exception("Recursion limit")

type SynNode =
  | Function of string
  | LitConst of double
  | SymConst of string
  | Register of string
  | RegAssign of string
  | UserQuery of string
  | IfElse of SynNode list * SynNode list
  | DoUntil of SynNode list * SynNode list
  | DoWhile of SynNode list * SynNode list
  | ForLoop of SynNode list

type ICalcFn =
  abstract member Execute: Calculator * int -> unit

and Calculator(stackLimit: int) =
  let mutable significant = 15
  let mutable epsilon = 1e-15
  let mutable loopLimit = 200000
  let mutable recurLimit = 50
  let mutable lastAnswer = 0.0

  let memory = Dictionary<string, double>()
  let funcs = Dictionary<string, ICalcFn>()
  let theStack = Stack<double>()

  new() = Calculator(1000)
  
  member this.SignificantFigures
    with get() = significant
    and set(value) =
      if value > 15 || value < 1 then
        raise (new ArgumentOutOfRangeException())
      significant <- value

  member this.ComparisonEpsilon
    with get() = epsilon
    and set(value) =
      if value < 1e-15 || value > 0.1 then
        raise (new ArgumentOutOfRangeException())
      epsilon <- value

  member this.IterationLimit
    with get() = loopLimit
    and set(value) =
      if value < 0 || value > 1000000 then
        raise (new ArgumentOutOfRangeException())
      loopLimit <- value

  member this.RecursionLimit
    with get() = recurLimit
    and set(value) =
      if value < 0 || value > 150 then
        raise (new ArgumentOutOfRangeException())
      recurLimit <- value

  member this.LastAnswer = lastAnswer

  member this.Stack
    with get() =
      if theStack.Count < 1 then
        raise (new CalcStackError(true))
      theStack.Pop()
    and set(value) =
      if theStack.Count >= stackLimit then
        raise (new CalcStackError(false))
      theStack.Push(value)

  member this.Memory
    with get(x) =
      let (ok,v) = memory.TryGetValue(x)
      if ok then v else 0.0
    and set(x) value =
      memory.[x] <- value

  member this.Function
    with get(x) =
      let (ok,v) = funcs.TryGetValue(x)
      if ok then v else raise (new CalcFunctionError(x))

  member this.SetNativeFunc(name: string, func: Action<_>) =
    funcs.["L"+name] <- NativeFn(func)
    
  member this.SetConstant(name: string, value: double) =
    memory.["."+name] <- value

  member this.SetUserFunc(name: string, func: CompFn) =
    funcs.["U"+name] <- func

  member this.RemoveUserFunc(name: string) =
    funcs.Remove("U"+name) |> ignore

  member this.Compile(code: string, paramNum: int) =
    this.Compile(code.Split(' '), paramNum)

  member this.Compile(tokens: string array, paramNum: int) =
    let (|StartsWithDrop|_|) patt (input: string) =
      if input.StartsWith(patt) then Some(input.Substring(patt.Length)) else None
    let (|StartsWithKeep|_|) patt (input: string) =
      if input.StartsWith(patt) then Some(input) else None

    let rec pTok tks =
      match tks with
      | (StartsWithDrop "#" x) :: xs ->
        let (ok,v) = Double.TryParse(x)
        if ok then (LitConst(v),xs)
        else raise (new CalcSyntaxError())
      | (StartsWithKeep "." x) :: xs ->
        (SymConst(x),xs)
      | (StartsWithKeep "$" x) :: xs ->
        (Register(x),xs)
      | "@set" :: (StartsWithKeep "$" x) :: xs ->
        (RegAssign(x),xs)
      | (StartsWithDrop "@read:" x) :: xs ->
        (UserQuery(x),xs)
      | "@if" :: xs ->
        loopIf [] [] false xs
      | "@for" :: xs ->
        loopFor [] xs
      | "@do" :: xs ->
        loopDo [] [] false false xs
      | (StartsWithKeep "L" x) :: xs
      | (StartsWithKeep "U" x) :: xs ->
        (Function(x),xs)
      | _ ->
        raise (new CalcSyntaxError())
    and loopIf a1 a2 f tks =
      match tks with
      | "@else" :: xs ->
        loopIf a1 a2 true xs
      | "@then" :: xs ->
        let (a1,a2) = (List.rev a1, List.rev a2)
        (IfElse(a1,a2),xs)
      | (_ :: _) as yy ->
        let (x',xs') = pTok yy
        if f then
          loopIf a1 (x' :: a2) f xs'
        else
          loopIf (x' :: a1) a2 f xs'
      | [] -> raise (new CalcSyntaxError())
    and loopDo a1 a2 f p tks =
      match tks with
      | "@loop" :: xs ->
        let (a1,a2) = (List.rev a1, List.rev a2)
        if p then
          (DoWhile(a1,a2),xs)
        else
          (DoUntil(a1,a2),xs)
      | "@while" :: xs ->
        loopDo a1 a2 true true xs
      | "@until" :: xs ->
        loopDo a1 a2 true false xs
      | (_ :: _) as yy ->
        let (x',xs') = pTok yy
        if f then
          loopDo a1 (x' :: a2) f p xs'
        else
          loopDo (x' :: a1) a2 f p xs'
      | [] -> raise (new CalcSyntaxError())
    and loopFor a tks =
      match tks with
      | "@loop" :: xs ->
        let a = List.rev a
        (ForLoop(a),xs)
      | (_ :: _) as yy ->
        let (x',xs') = pTok yy
        loopFor (x' :: a) xs'
      | [] -> raise (new CalcSyntaxError())

    let rec loop acc tks =
      match tks with
      | (_ :: _) as yy ->
        let (x,xs) = pTok yy
        loop (x :: acc) xs
      | [] -> acc

    let tokens =
      Array.toList tokens
      |> List.filter (fun x -> x.Length > 0)
      |> loop []
      |> List.rev
    new CompFn(tokens, Math.Max(0, paramNum))

  member this.Execute(func: ICalcFn) =
    let figures (x: float) (n: int) =
      match x with
      | 0.0 -> 0.0
      | x when Double.IsNaN(x) -> x
      | x when Double.IsInfinity(x) -> x
      | x ->
        let power = round (log10 (abs x))
        let log = 10.0 ** (power + 1.0 - float n)
        log * Math.Round(x / log)

    theStack.Clear()
    func.Execute(this, 1)
    let result = if theStack.Count > 0 then this.Stack else 0.0
    let result = figures result significant
    lastAnswer <- result
    result

and CompFn (ops: SynNode list, paramNum: int) =
  new(ops: SynNode list) = CompFn(ops, 0)

  member this.ParamCount = paramNum

  member this.Execute(calc: Calculator, recur: int) =
    let rec saveReg acc idx =
      match idx with
      | x when x > 0 ->
        let reg = "$g" + (string idx)
        let value = calc.Memory(reg)
        calc.Memory(reg) <- calc.Stack
        saveReg (value :: acc) (idx-1)
      | _ -> acc

    let rec placeReg acc idx =
      match acc with
      | x :: xs ->
        let reg = "$g" + (string idx)
        calc.Memory(reg) <- x
        placeReg xs (idx+1)
      | [] -> ()

    let rec loop tks =
      match tks with
      | LitConst(x) :: xs ->
        calc.Stack <- x
        loop xs
      | Function(x) :: xs ->
        calc.Function(x).Execute(calc, recur+1)
        loop xs
      | SymConst(x) :: xs
      | Register(x) :: xs ->
        calc.Stack <- calc.Memory(x)
        loop xs
      | RegAssign(x) :: xs ->
        calc.Memory(x) <- calc.Stack
        loop xs
      | UserQuery(_) :: xs ->
        loop xs
      | IfElse(x,y) :: xs ->
        let f = calc.Stack
        if abs f >= 0.5 then
          loop x
        else
          loop y
        loop xs
      | DoUntil(x,y) :: xs ->
        let rec iter lim =
          if lim > calc.IterationLimit then
            raise (new CalcMaxIterationError())
          loop x
          loop y
          let f = calc.Stack
          if abs f < 0.5 then
            iter (lim+1)
        iter 1
        loop xs
      | DoWhile(x,y) :: xs ->
        let rec iter lim =
          if lim > calc.IterationLimit then
            raise (new CalcMaxIterationError())
          loop y
          let f = calc.Stack
          if abs f >= 0.5 then
            loop x
            iter (lim+1)
        iter 1
        loop xs
      | ForLoop(x) :: xs ->
        let rec iter a i lim =
          if lim > calc.IterationLimit then
            raise (new CalcMaxIterationError())
          calc.Stack <- float a
          loop x
          if i > 1 then
            iter (a+1) (i-1) (lim+1)
        let (b,a) = (int calc.Stack, int calc.Stack)
        iter a b 1
        loop xs
      | [] -> ()

    if recur > calc.RecursionLimit then
      raise (new CalcMaxRecursionError())
    let rg = saveReg [] paramNum
    loop ops
    placeReg rg 1

  interface ICalcFn with
    member this.Execute(calc, recur) =
      this.Execute(calc, recur)

and NativeFn (func: Action<Calculator>) =
  member this.Execute(calc: Calculator) =
    func.Invoke(calc)

  interface ICalcFn with
    member this.Execute(calc, _) =
      this.Execute(calc)