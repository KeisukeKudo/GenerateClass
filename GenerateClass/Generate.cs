using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace GenerateClass {
    class Generate {

        #region Entityクラス作成

        /// <summary>
        /// テーブルに対応するクラスを作成する
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="className"></param>
        /// <param name="tableInfo"></param>
        public void GenerateEntity(string nameSpace, string className, List<TableInfo> tableInfo) {

            var compileUnit = new CodeCompileUnit();

            //名前空間を設定
            var name = new CodeNamespace(nameSpace);

            compileUnit.Namespaces.Add(name);

            //クラス定義 引数にはクラス名を設定
            var classType = new CodeTypeDeclaration(className);

            foreach (var t in tableInfo) {
                //propertyを定義
                var field = new CodeMemberField {
                    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    Name = t.ColumnName + " { get; set; }",
                    Type = new CodeTypeReference(this.GetPropertyDataType(t.DataType)),
                };

                classType.Members.Add(field);
            }

            name.Types.Add(classType);

            var provider = new CSharpCodeProvider();

            //CSharpCodeProvider().FileExtensionで｢cs｣拡張子を取得できます
            var fileName = $"{ classType.Name }.{ provider.FileExtension }";

            //Entityクラスを出力
            using (var writer = File.CreateText(fileName)) {
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
            }

            //各プロパティ末尾のセミコロン削除
            this.DeletePropertysEndComma(fileName);

        }

        /// <summary>
        /// SQLiteのデータ型に合った型を返す
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private Type GetPropertyDataType(string dataType) {

            //今回はBLOB型を無視します｡
            switch (dataType) {
                case "TEXT":
                    return typeof(string);
                case "INTEGER":
                    return typeof(int);
                case "REAL":
                    return typeof(double);
                default:
                    throw new ArgumentException("型が不明です: " + dataType);
            }

        }

        /// <summary>
        /// ｢public int id { get; set; };｣のように末尾にセミコロンが付いてしまうので削除する
        /// </summary>
        /// <param name="fileName"></param>
        private void DeletePropertysEndComma(string fileName) {

            //ファイルを読込み､波括弧末尾のセミコロンを削除
            string fileDetail = File.ReadAllText(fileName).Replace("};", "}");

            //再度ファイルに書き出す
            using (var writer = new StreamWriter(fileName)) {
                writer.Write(fileDetail);
            }

        }

        #endregion
        
        #region exe作成

        public void GenerateHelloWorldExe() {
            var compileUnit = new CodeCompileUnit();

            //名前空間を設定
            var name = new CodeNamespace("Namespace1");

            //Systemをインポート
            name.Imports.Add(new CodeNamespaceImport("System"));

            //クラス定義 引数にはクラス名を設定
            var classType = new CodeTypeDeclaration("HelloWorld");

            //public static void Main() を作成
            var method = new CodeEntryPointMethod();

            //Console.WriteLine("Hello World!"); を定義
            var writeLine = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("Console"), "WriteLine",
                new CodePrimitiveExpression("Hello World!")
            );

            //Console.ReadKey(); を定義
            var readKey = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("Console"), "ReadKey"
            );

            //上記で定義した処理をMain()に追加する
            method.Statements.Add(new CodeExpressionStatement(writeLine));
            method.Statements.Add(new CodeExpressionStatement(readKey));

            classType.Members.Add(method);

            name.Types.Add(classType);

            compileUnit.Namespaces.Add(name);

            var provider = new CSharpCodeProvider();

            //確認のため生成コードをコンソールへ出力
            provider.GenerateCodeFromCompileUnit(compileUnit, Console.Out, new CodeGeneratorOptions());

            //実行ファイル(HelloWorld.exe)を作成
            var param = new CompilerParameters { GenerateExecutable = true, OutputAssembly = "HelloWorld.exe" };
            CompilerResults result = provider.CompileAssemblyFromDom(param, new CodeCompileUnit[] { compileUnit });

        }

        #endregion

    }
}
