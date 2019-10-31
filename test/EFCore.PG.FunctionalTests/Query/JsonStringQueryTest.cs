using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Npgsql.EntityFrameworkCore.PostgreSQL.Extensions;
using Npgsql.EntityFrameworkCore.PostgreSQL.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Query
{
    public class JsonStringQueryTest : IClassFixture<JsonStringQueryTest.JsonStringQueryFixture>
    {
        JsonStringQueryFixture Fixture { get; }

        public JsonStringQueryTest(JsonStringQueryFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestSqlLoggerFactory.Clear();
            Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [Fact]
        public void Roundtrip()
        {
            using (var ctx = Fixture.CreateContext())
            {
                var entity = ctx.JsonEntities.Single(e => e.Id == 1);
                PerformAsserts(entity.CustomerJsonb);
                PerformAsserts(entity.CustomerJson);
            }

            static void PerformAsserts(string customerText)
            {
                var customer = JsonDocument.Parse(customerText).RootElement;
                Assert.Equal("Joe", customer.GetProperty("Name").GetString());
                Assert.Equal(25, customer.GetProperty("Age").GetInt32());
                var order1 = customer.GetProperty("Orders")[0];
                Assert.Equal(99.5m, order1.GetProperty("Price").GetDecimal());
                Assert.Equal("Some address 1", order1.GetProperty("ShippingAddress").GetString());
                Assert.Equal(new DateTime(2019, 10, 1), order1.GetProperty("ShippingDate").GetDateTime());
                var order2 = customer.GetProperty("Orders")[1];
                Assert.Equal(23, order2.GetProperty("Price").GetDecimal());
                Assert.Equal("Some address 2", order2.GetProperty("ShippingAddress").GetString());
                Assert.Equal(new DateTime(2019, 10, 10), order2.GetProperty("ShippingDate").GetDateTime());
            }
        }

        [Fact]
        public void Literal()
        {
            using (var ctx = Fixture.CreateContext())
            {
                Assert.Empty(ctx.JsonEntities.Where(e => e.CustomerJsonb == @"{""Name"":""Test customer"",""Age"":80,""IsVip"":false,""Statistics"":null,""Orders"":null}"));

                AssertSql(
                    @"SELECT j.""Id"", j.""CustomerJson"", j.""CustomerJsonb""
FROM ""JsonEntities"" AS j
WHERE (j.""CustomerJsonb"" = '{""Name"":""Test customer"",""Age"":80,""IsVip"":false,""Statistics"":null,""Orders"":null}') AND (j.""CustomerJsonb"" IS NOT NULL)");
            }
        }

        [Fact]
        public void Parameter()
        {
            using (var ctx = Fixture.CreateContext())
            {
                var expected = ctx.JsonEntities.Find(1).CustomerJsonb;
                var actual = ctx.JsonEntities.Single(e => e.CustomerJsonb == expected).CustomerJsonb;
                Assert.Equal(actual, expected);

                AssertSql(
                    @"@__p_0='1'

SELECT j.""Id"", j.""CustomerJson"", j.""CustomerJsonb""
FROM ""JsonEntities"" AS j
WHERE j.""Id"" = @__p_0
LIMIT 1",
                    //
                    @"@__expected_0='{""Age"": 25
""Name"": ""Joe""
""IsVip"": false
""Orders"": [{""Price"": 99.5
""ShippingDate"": ""2019-10-01""
""ShippingAddress"": ""Some address 1""}
{""Price"": 23
""ShippingDate"": ""2019-10-10""
""ShippingAddress"": ""Some address 2""}]
""Statistics"": {""Nested"": {""IntArray"": [3
4]
""SomeProperty"": 10}
""Visits"": 4
""Purchases"": 3}}' (DbType = Object)

SELECT j.""Id"", j.""CustomerJson"", j.""CustomerJsonb""
FROM ""JsonEntities"" AS j
WHERE (j.""CustomerJsonb"" = @__expected_0) AND (j.""CustomerJsonb"" IS NOT NULL)
LIMIT 2");
            }
        }

        #region Functions

//        [Fact]
//        public void JsonContains_with_json_element()
//        {
//            using (var ctx = Fixture.CreateContext())
//            {
//                var element = JsonDocument.Parse(@"{""Name"": ""Joe"", ""Age"": 25}").RootElement;
//                var count = ctx.JsonEntities.Count(e =>
//                    EF.Functions.JsonContains(e.Customer, element));
//                Assert.Equal(1, count);
//
//                AssertSql(
//                    @"@__element_1='{""Name"": ""Joe""
//""Age"": 25}' (DbType = Object)
//
//SELECT COUNT(*)::INT
//FROM ""JsonbEntities"" AS j
//WHERE (j.""Customer"" @> @__element_1)");
//            }
//        }

        [Fact]
        public void JsonContains_with_string()
        {
            using (var ctx = Fixture.CreateContext())
            {
                var count = ctx.JsonEntities.Count(e =>
                    EF.Functions.JsonContains(e.CustomerJsonb, @"{""Name"": ""Joe"", ""Age"": 25}"));
                Assert.Equal(1, count);

                AssertSql(
                    @"SELECT COUNT(*)::INT
FROM ""JsonEntities"" AS j
WHERE (j.""CustomerJsonb"" @> '{""Name"": ""Joe"", ""Age"": 25}')");
            }
        }

        [Fact]
        public void JsonContained_with_json_element()
        {
            using (var ctx = Fixture.CreateContext())
            {
                var element = JsonDocument.Parse(@"{""Name"": ""Joe"", ""Age"": 25}").RootElement;
                var count = ctx.JsonEntities.Count(e =>
                    EF.Functions.JsonContained(element, e.CustomerJsonb));
                Assert.Equal(1, count);

                AssertSql(
                    @"@__element_1='{""Name"": ""Joe""
""Age"": 25}' (DbType = Object)

SELECT COUNT(*)::INT
FROM ""JsonEntities"" AS j
WHERE (@__element_1 <@ j.""CustomerJsonb"")");
            }
        }

        [Fact]
        public void JsonContained_with_string()
        {
            using (var ctx = Fixture.CreateContext())
            {
                var count = ctx.JsonEntities.Count(e =>
                    EF.Functions.JsonContained(@"{""Name"": ""Joe"", ""Age"": 25}", e.CustomerJsonb));
                Assert.Equal(1, count);

                AssertSql(
                    @"SELECT COUNT(*)::INT
FROM ""JsonEntities"" AS j
WHERE ('{""Name"": ""Joe"", ""Age"": 25}' <@ j.""CustomerJsonb"")");
            }
        }

        [Fact]
        public void JsonExists()
        {
            using (var ctx = Fixture.CreateContext())
            {
                var count = ctx.JsonEntities.Count(e =>
                    EF.Functions.JsonExists(e.CustomerJsonb, "Age"));
                Assert.Equal(2, count);

                AssertSql(
                    @"SELECT COUNT(*)::INT
FROM ""JsonEntities"" AS j
WHERE (j.""CustomerJsonb"" ? 'Age')");
            }
        }

        [Fact]
        public void JsonExistAny()
        {
            using (var ctx = Fixture.CreateContext())
            {
                var count = ctx.JsonEntities.Count(e =>
                    EF.Functions.JsonExistAny(e.CustomerJsonb, "foo", "Age"));
                Assert.Equal(2, count);

                AssertSql(
                    @"SELECT COUNT(*)::INT
FROM ""JsonEntities"" AS j
WHERE (j.""CustomerJsonb"" ?| ARRAY['foo','Age']::text[])");
            }
        }

        [Fact]
        public void JsonExistAll()
        {
            using (var ctx = Fixture.CreateContext())
            {
                var count = ctx.JsonEntities.Count(e =>
                    EF.Functions.JsonExistAll(e.CustomerJsonb, "foo", "Age"));
                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)::INT
FROM ""JsonEntities"" AS j
WHERE (j.""CustomerJsonb"" ?& ARRAY['foo','Age']::text[])");
            }
        }

        #endregion Functions

        #region Support

        void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class JsonStringQueryContext : PoolableDbContext
        {
            public DbSet<JsonEntity> JsonEntities { get; set; }

            public JsonStringQueryContext(DbContextOptions options) : base(options) {}

            public static void Seed(JsonStringQueryContext context)
            {
                const string customer1 = @"
                {
                    ""Name"": ""Joe"",
                    ""Age"": 25,
                    ""IsVip"": false,
                    ""Statistics"":
                    {
                        ""Visits"": 4,
                        ""Purchases"": 3,
                        ""Nested"":
                        {
                            ""SomeProperty"": 10,
                            ""IntArray"": [3, 4]
                        }
                    },
                    ""Orders"":
                    [
                        {
                            ""Price"": 99.5,
                            ""ShippingAddress"": ""Some address 1"",
                            ""ShippingDate"": ""2019-10-01""
                        },
                        {
                            ""Price"": 23,
                            ""ShippingAddress"": ""Some address 2"",
                            ""ShippingDate"": ""2019-10-10""
                        }
                    ]
                }";

                const string customer2 = @"
                {
                    ""Name"": ""Moe"",
                    ""Age"": 35,
                    ""IsVip"": true,
                    ""Statistics"":
                    {
                        ""Visits"": 20,
                        ""Purchases"": 25,
                        ""Nested"":
                        {
                            ""SomeProperty"": 20,
                            ""IntArray"": [5, 6]
                        }
                    },
                    ""Orders"":
                    [
                        {
                            ""Price"": 5,
                            ""ShippingAddress"": ""Moe's address"",
                            ""ShippingDate"": ""2019-11-03""
                        }
                    ]
                }";

                context.JsonEntities.AddRange(
                    new JsonEntity { Id = 1, CustomerJsonb = customer1, CustomerJson = customer1 },
                    new JsonEntity { Id = 2, CustomerJsonb = customer2, CustomerJson = customer2 });
                context.SaveChanges();
            }
        }

        public class JsonEntity
        {
            public int Id { get; set; }

            [Column(TypeName = "jsonb")]
            public string CustomerJsonb { get; set; }
            [Column(TypeName = "json")]
            public string CustomerJson { get; set; }
        }

        public class JsonStringQueryFixture : SharedStoreFixtureBase<JsonStringQueryContext>
        {
            protected override string StoreName => "JsonStringQueryTest";
            protected override ITestStoreFactory TestStoreFactory => NpgsqlTestStoreFactory.Instance;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
            protected override void Seed(JsonStringQueryContext context) => JsonStringQueryContext.Seed(context);
        }

        #endregion
    }
}
