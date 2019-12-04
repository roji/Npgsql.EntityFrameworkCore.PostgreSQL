﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal
{
    public class NpgsqlFuzzyStringMatchMethodTranslator : IMethodCallTranslator
    {
        static readonly Dictionary<MethodInfo, string> Functions = new Dictionary<MethodInfo, string>
        {
            [GetRuntimeMethod(nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchSoundex), new[] { typeof(DbFunctions), typeof(string) })] = "soundex",
            [GetRuntimeMethod(nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchDifference), new[] { typeof(DbFunctions), typeof(string), typeof(string) })] = "difference",
            [GetRuntimeMethod(nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchLevenshtein), new[] { typeof(DbFunctions), typeof(string), typeof(string) })] = "levenshtein",
            [GetRuntimeMethod(nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchLevenshtein), new[] { typeof(DbFunctions), typeof(string), typeof(string), typeof(int), typeof(int), typeof(int) })] = "levenshtein",
            [GetRuntimeMethod(nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchLevenshteinLessEqual), new[] { typeof(DbFunctions), typeof(string), typeof(string), typeof(int) })] = "levenshtein_less_equal",
            [GetRuntimeMethod(nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchLevenshteinLessEqual), new[] { typeof(DbFunctions), typeof(string), typeof(string), typeof(int), typeof(int), typeof(int), typeof(int) })] = "levenshtein_less_equal",
            [GetRuntimeMethod(nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchMetaphone), new[] { typeof(DbFunctions), typeof(string), typeof(int) })] = "metaphone",
            [GetRuntimeMethod(nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchDoubleMetaphone), new[] { typeof(DbFunctions), typeof(string) })] = "dmetaphone",
            [GetRuntimeMethod(nameof(NpgsqlFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchDoubleMetaphoneAlt), new[] { typeof(DbFunctions), typeof(string) })] = "dmetaphone_alt"
        };

        static MethodInfo GetRuntimeMethod(string name, params Type[] parameters)
            => typeof(NpgsqlFuzzyStringMatchDbFunctionsExtensions).GetRuntimeMethod(name, parameters);

        readonly NpgsqlSqlExpressionFactory _sqlExpressionFactory;

        public NpgsqlFuzzyStringMatchMethodTranslator(NpgsqlSqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource typeMappingSource)
            => _sqlExpressionFactory = sqlExpressionFactory;

        /// <inheritdoc />
        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
            => Functions.TryGetValue(method, out var function)
                ? _sqlExpressionFactory.Function(function, arguments.Skip(1), method.ReturnType)
                : null;
    }
}