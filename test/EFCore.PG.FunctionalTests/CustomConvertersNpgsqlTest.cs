﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Npgsql.EntityFrameworkCore.PostgreSQL.TestUtilities;
using Xunit;

namespace Npgsql.EntityFrameworkCore.PostgreSQL
{
    public class CustomConvertersNpgsqlTest : CustomConvertersTestBase<CustomConvertersNpgsqlTest.CustomConvertersNpgsqlFixture>
    {
        public CustomConvertersNpgsqlTest(CustomConvertersNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        // Disabled: PostgreSQL is case-sensitive
        public override void Can_insert_and_read_back_with_case_insensitive_string_key() {}

        [Fact(Skip="https://github.com/aspnet/EntityFrameworkCore/issues/14159")]
        public override void Can_query_using_any_data_type_nullable_shadow() {}

        public class CustomConvertersNpgsqlFixture : CustomConvertersFixtureBase
        {
            public override bool StrictEquality => true;

            public override bool SupportsAnsi => false;

            public override bool SupportsUnicodeToAnsiConversion => true;

            public override bool SupportsLargeStringComparisons => true;

            protected override ITestStoreFactory TestStoreFactory => NpgsqlTestStoreFactory.Instance;

            public override bool SupportsBinaryKeys => true;

            public override bool SupportsDecimalComparisons => true;

            public override DateTime DefaultDateTime => new DateTime();

            // TODO: Remove the following after https://github.com/aspnet/EntityFrameworkCore/pull/11587 is merged
            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder.Entity<MaxLengthDataTypes>(b => b.Property(e => e.ByteArray9000)
                    .HasMaxLength(LongStringLength * 2));
            }
        }
    }
}
