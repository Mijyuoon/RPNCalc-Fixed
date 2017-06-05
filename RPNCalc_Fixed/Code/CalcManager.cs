using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RPNCalc.Controls;
using RPNCalc.Utils;
using RPNCalcLib;

namespace RPNCalc {
    public class CalcManager {
        private Calculator calc;
        private ObservableCollection<Function> fnList;

        public CalcManager(Calculator calc) {
            this.calc = calc;
            fnList = new ObservableCollection<Function>();

            SignificantFigures = 10;
            ComparisonEpsilon  = 13;
            IterationLimit = 200;
            RecursionLimit = 5;
        }

        public int SignificantFigures {
            get { return calc.SignificantFigures; }
            set { calc.SignificantFigures = value; }
        }

        public double ComparisonEpsilon {
            get { return -Math.Log10(calc.ComparisonEpsilon); }
            set { calc.ComparisonEpsilon = Math.Pow(10.0, -value); }
        }

        public int IterationLimit {
            get { return calc.IterationLimit / 1000; }
            set { calc.IterationLimit = value * 1000; }
        }

        public int RecursionLimit {
            get { return calc.RecursionLimit / 10; }
            set { calc.RecursionLimit = value * 10; }
        }

        public ObservableCollection<Function> Functions {
            get { return fnList; }
        }

        public void ClearAllFunctions() {
            foreach(var fn in fnList)
                calc.RemoveUserFunc(fn.Name);
            fnList.Clear();
        }

        public class Function : PropertyNotifier {
            private string name;
            private string descr;
            private CalcCommand[] code;
            private int paramNum;

            public string Name {
                get { return name; }
                set { SetProperty(ref name, value); }
            }
            public string Descr {
                get { return descr; }
                set { SetProperty(ref descr, value); }
            }
            public int ParamNum {
                get { return paramNum; }
                set { SetProperty(ref paramNum, value); }
            }
            public CalcCommand[] Code {
                get { return code; }
                set { SetProperty(ref code, value); }
            }

            public CompFn Compile(Calculator calc) {
                var toks = code
                    .Select(x => x.Action)
                    .ToArray();
                return calc.Compile(toks, paramNum);
            }
            public void Serialize(BinaryWriter writer) {
                writer.Write(name);
                writer.Write(descr);
                writer.Write(paramNum);

                writer.Write(code.Length);
                foreach(var ob in code)
                    ob.Serialize(writer);
            }
            public static Function Deserialize(BinaryReader reader) {
                var name = reader.ReadString();
                var descr = reader.ReadString();
                var paramNum = reader.ReadInt32();

                var len = reader.ReadInt32();
                var code = new CalcCommand[len];
                for(int i = 0; i < len; i++)
                    code[i] = CalcCommand.Deserialize(reader);

                return new Function() {
                    name = name,
                    descr = descr,
                    paramNum = paramNum,
                    code = code,
                };
            }
        }
    }
}
