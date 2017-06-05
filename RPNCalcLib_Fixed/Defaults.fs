namespace RPNCalcLib

type M = System.Math

module CalcDefaults =
  let iclamp x a b =
    match int x with
    | x when x < a -> a
    | x when x > b -> b
    | x -> x

  let boolify f = if f then 1.0 else 0.0

  let BasicOperators(calc: Calculator) =
    calc.SetNativeFunc("add", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- a + b)
    calc.SetNativeFunc("sub", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- a - b)
    calc.SetNativeFunc("mul", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- a * b)
    calc.SetNativeFunc("div", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- a / b)
    calc.SetNativeFunc("mod", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- a % b)

    calc.SetNativeFunc("neg", fun x -> let a = x.Stack in x.Stack <- -a)
    calc.SetNativeFunc("inv", fun x -> let a = x.Stack in x.Stack <- 1.0 / a)
    calc.SetNativeFunc("abs", fun x -> let a = x.Stack in x.Stack <- abs a)
    calc.SetNativeFunc("floor", fun x -> let a = x.Stack in x.Stack <- floor a)
    calc.SetNativeFunc("ceil", fun x -> let a = x.Stack in x.Stack <- ceil a)
    calc.SetNativeFunc("round", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- M.Round(a, iclamp b 0 15))

  let AdvancedOperators(calc: Calculator) =
    calc.SetNativeFunc("pow", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- a ** b)
    calc.SetNativeFunc("exp", fun x -> let a = x.Stack in x.Stack <- exp a)
    calc.SetNativeFunc("sqrt", fun x -> let a = x.Stack in x.Stack <- sqrt a)
    calc.SetNativeFunc("nthrt", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- a ** (1.0 / b))
    calc.SetNativeFunc("ln", fun x -> let a = x.Stack in x.Stack <- log a)
    calc.SetNativeFunc("log", fun x -> let a = x.Stack in x.Stack <- log10 a)
    
    calc.SetNativeFunc("sin", fun x -> let a = x.Stack in x.Stack <- sin a)
    calc.SetNativeFunc("cos", fun x -> let a = x.Stack in x.Stack <- cos a)
    calc.SetNativeFunc("tan", fun x -> let a = x.Stack in x.Stack <- tan a)
    calc.SetNativeFunc("asin", fun x -> let a = x.Stack in x.Stack <- asin a)
    calc.SetNativeFunc("acos", fun x -> let a = x.Stack in x.Stack <- acos a)
    calc.SetNativeFunc("atan", fun x -> let a = x.Stack in x.Stack <- atan a)

    calc.SetNativeFunc("sinh", fun x -> let a = x.Stack in x.Stack <- sinh a)
    calc.SetNativeFunc("cosh", fun x -> let a = x.Stack in x.Stack <- cosh a)
    calc.SetNativeFunc("tanh", fun x -> let a = x.Stack in x.Stack <- tanh a)
    calc.SetNativeFunc("asinh", fun x -> let a = x.Stack in x.Stack <- log (a + sqrt (a * a + 1.0)))
    calc.SetNativeFunc("acosh", fun x -> let a = x.Stack in x.Stack <- log (a + sqrt (a * a - 1.0)))
    calc.SetNativeFunc("atanh", fun x -> let a = x.Stack in x.Stack <- 0.5 * log ((1.0 + a) / (1.0 - a)))

  let Programming(calc: Calculator) =
    let compare (a: double) b =
      let err = calc.ComparisonEpsilon
      if abs (a-b) < err then
        boolify true
      elif abs a > abs b then
        boolify (abs ((a-b)/a) < err)
      else
        boolify (abs ((a-b)/b) < err)

    calc.SetNativeFunc("dup", fun x -> let a = x.Stack in x.Stack <- a; x.Stack <- a)
    calc.SetNativeFunc("swap", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- b; x.Stack <- a)
    calc.SetNativeFunc("drop", fun x -> x.Stack |> ignore)
    calc.SetNativeFunc("over", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- a; x.Stack <- b; x.Stack <- a)

    calc.SetNativeFunc("eq", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- (compare a b))
    calc.SetNativeFunc("neq", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- 1.0 - (compare a b))
    calc.SetNativeFunc("gt", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- boolify (a > b))
    calc.SetNativeFunc("lt", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- boolify (a < b))
    
    calc.SetNativeFunc("not", fun x -> let a = x.Stack in x.Stack <- boolify (abs a < 0.5))
    calc.SetNativeFunc("and", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- boolify ((abs a >= 0.5) && (abs b >= 0.5)))
    calc.SetNativeFunc("or", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- boolify ((abs a >= 0.5) || (abs b >= 0.5)))

  let Constants(calc: Calculator) =
    calc.SetConstant("c", 299792458.0)
    calc.SetConstant("G", 6.67408e-11)
    calc.SetConstant("g0", 9.80665)
    calc.SetConstant("h", 6.626070040e-34)
    calc.SetConstant("hbar", 1.054571800e-34)
    calc.SetConstant("e0", 1.6021766208e-19)
    calc.SetConstant("me", 9.10938356e-31)
    calc.SetConstant("ke", 8.987551787e+9)
    calc.SetConstant("eps0", 8.854187817e-12)
    calc.SetConstant("mu0", 12.566370614e-7)
    calc.SetConstant("NA", 6.022140857e+23)
    calc.SetConstant("kB", 1.38064852e-23)
    calc.SetConstant("R", 8.314459861)

  let Miscelaneous(calc: Calculator) =
    calc.SetConstant("pi", M.PI)
    calc.SetNativeFunc("ans", fun x -> x.Stack <- x.LastAnswer)
    calc.SetNativeFunc("mag", fun x -> let (b,a) = (x.Stack, x.Stack) in x.Stack <- a * (10.0 ** b))
    calc.SetNativeFunc("fact", fun x -> let a = x.Stack in x.Stack <- Seq.reduce (*) (seq { 1.0 .. (max 1.0 a) }))
