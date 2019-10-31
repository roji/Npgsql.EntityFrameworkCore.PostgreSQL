using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using Xunit.Abstractions;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Query
{
    // ReSharper disable once UnusedMember.Global
    public class SpatialQueryNpgsqlGeographyTest : SpatialQueryTestBase<SpatialQueryNpgsqlGeographyFixture>
    {
        // ReSharper disable once UnusedParameter.Local
        public SpatialQueryNpgsqlGeographyTest(SpatialQueryNpgsqlGeographyFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
            => Fixture.TestSqlLoggerFactory.Clear();

        protected override bool AssertDistances
            => false;

        public override async Task Area(bool isAsync)
        {
            await base.Area(isAsync);

            AssertSql(
                @"SELECT p.""Id"", ST_Area(p.""Polygon"") AS ""Area""
FROM ""PolygonEntity"" AS p");
        }

        public override async Task AsBinary(bool isAsync)
        {
            await base.AsBinary(isAsync);

//            AssertSql(
//                @"SELECT p.""Id"", ST_AsBinary(p.""Point"") AS ""Binary""
//FROM ""PointEntity"" AS p");
        }

        public override async Task AsText(bool isAsync)
        {
            await base.AsText(isAsync);

//            AssertSql(
//                @"SELECT p.""Id"", ST_AsText(p.""Point"") AS ""Text""
//FROM ""PointEntity"" AS p");
        }

        public override async Task Buffer(bool isAsync)
        {
            await base.Buffer(isAsync);

//            AssertSql(
//                @"SELECT p.""Id"", ST_Buffer(p.""Polygon"", 1.0) AS ""Buffer""
//FROM ""PolygonEntity"" AS p");
        }

        public override async Task Buffer_quadrantSegments(bool isAsync)
        {
            await base.Buffer_quadrantSegments(isAsync);

//            AssertSql(
//                @"SELECT p.""Id"", ST_Buffer(p.""Polygon"", 1.0, 8) AS ""Buffer""
//FROM ""PolygonEntity"" AS p");
        }

        public override async Task Centroid(bool isAsync)
        {
            await base.Centroid(isAsync);

            AssertSql(
                @"SELECT p.""Id"", ST_Centroid(p.""Polygon"") AS ""Centroid""
FROM ""PolygonEntity"" AS p");
        }

        public override async Task Distance(bool isAsync)
        {
            await base.Distance(isAsync);

//            AssertSql(
//                @"@__point_0='POINT (0 1)' (DbType = Object)
//
//SELECT p.""Id"", ST_Distance(p.""Point"", @__point_0) AS ""Distance""
//FROM ""PointEntity"" AS p");
        }

        public override async Task GeometryType(bool isAsync)
        {
            // PostGIS returns "POINT", NTS returns "Point"
            await AssertQuery(
                isAsync,
                es => es.Set<PointEntity>().Select(
                    e => new { e.Id, GeometryType = e.Point == null ? null : e.Point.GeometryType.ToLower() }),
                x => x.Id);

//            AssertSql(
//                @"SELECT p.""Id"", LOWER(GeometryType(p.""Point"")) AS ""GeometryType""
//FROM ""PointEntity"" AS p");
        }

        public override async Task Intersection(bool isAsync)
        {
            await base.Intersection(isAsync);

//            AssertSql(
//                @"@__polygon_0='POLYGON ((0 0
//1 0
//1 1
//0 0))' (DbType = Object)
//
//SELECT p.""Id"", ST_Intersection(p.""Polygon"", @__polygon_0) AS ""Intersection""
//FROM ""PolygonEntity"" AS p");
        }

        public override async Task Intersects(bool isAsync)
        {
            await base.Intersects(isAsync);

//            AssertSql(
//                @"@__lineString_0='LINESTRING (0.5 -0.5
//0.5 0.5)' (DbType = Object)
//
//SELECT l.""Id"", ST_Intersects(l.""LineString"", @__lineString_0) AS ""Intersects""
//FROM ""LineStringEntity"" AS l");
        }

        public override async Task IsWithinDistance(bool isAsync)
        {
            await base.IsWithinDistance(isAsync);

//            AssertSql(
//                @"@__point_0='POINT (0 1)' (DbType = Object)
//
//SELECT p.""Id"", ST_DWithin(p.""Point"", @__point_0, 1.0) AS ""IsWithinDistance""
//FROM ""PointEntity"" AS p");
        }

        public override async Task Length(bool isAsync)
        {
            await base.Length(isAsync);

            AssertSql(
                @"SELECT l.""Id"", ST_Length(l.""LineString"") AS ""Length""
FROM ""LineStringEntity"" AS l");
        }

        public override async Task SRID(bool isAsync)
        {
            await base.SRID(isAsync);

            AssertSql(
                @"SELECT p.""Id"", ST_SRID(p.""Point"") AS ""SRID""
FROM ""PointEntity"" AS p");
        }

        public override async Task ToBinary(bool isAsync)
        {
            await base.ToBinary(isAsync);

//            AssertSql(
//                @"SELECT p.""Id"", ST_AsBinary(p.""Point"") AS ""Binary""
//FROM ""PointEntity"" AS p");
        }

        public override async Task ToText(bool isAsync)
        {
            await base.ToText(isAsync);

//            AssertSql(
//                @"SELECT p.""Id"", ST_AsText(p.""Point"") AS ""Text""
//FROM ""PointEntity"" AS p");
        }

        #region Not supported on geography

        public override Task Boundary(bool isAsync)                  => Task.CompletedTask;
        public override Task Contains(bool isAsync)                  => Task.CompletedTask;
        public override Task ConvexHull(bool isAsync)                => Task.CompletedTask;
        public override Task Crosses(bool isAsync)                   => Task.CompletedTask;
        public override Task Difference(bool isAsync)                => Task.CompletedTask;
        public override Task Dimension(bool isAsync)                 => Task.CompletedTask;
        public override Task Disjoint(bool isAsync)                  => Task.CompletedTask;
        public override Task EndPoint(bool isAsync)                  => Task.CompletedTask;
        public override Task Envelope(bool isAsync)                  => Task.CompletedTask;
        public override Task EqualsTopologically(bool isAsync)       => Task.CompletedTask;
        public override Task ExteriorRing(bool isAsync)              => Task.CompletedTask;
        public override Task GetGeometryN(bool isAsync)              => Task.CompletedTask;
        public override Task GetInteriorRingN(bool isAsync)          => Task.CompletedTask;
        public override Task GetPointN(bool isAsync)                 => Task.CompletedTask;
        public override Task ICurve_IsClosed(bool isAsync)           => Task.CompletedTask;
        public override Task IGeometryCollection_Count(bool isAsync) => Task.CompletedTask;
        public override Task IMultiCurve_IsClosed(bool isAsync)      => Task.CompletedTask;
        public override Task IsEmpty(bool isAsync)                   => Task.CompletedTask;
        public override Task IsRing(bool isAsync)                    => Task.CompletedTask;
        public override Task IsSimple(bool isAsync)                  => Task.CompletedTask;
        public override Task IsValid(bool isAsync)                   => Task.CompletedTask;
        public override Task Item(bool isAsync)                      => Task.CompletedTask;
        public override Task InteriorPoint(bool isAsync)             => Task.CompletedTask;
        public override Task LineString_Count(bool isAsync)          => Task.CompletedTask;
        public override Task M(bool isAsync)                         => Task.CompletedTask;
        public override Task NumGeometries(bool isAsync)             => Task.CompletedTask;
        public override Task NumInteriorRings(bool isAsync)          => Task.CompletedTask;
        public override Task NumPoints(bool isAsync)                 => Task.CompletedTask;
        public override Task OgcGeometryType(bool isAsync)           => Task.CompletedTask;
        public override Task Overlaps(bool isAsync)                  => Task.CompletedTask;
        public override Task PointOnSurface(bool isAsync)            => Task.CompletedTask;
        public override Task Relate(bool isAsync)                    => Task.CompletedTask;
        public override Task Reverse(bool isAsync)                   => Task.CompletedTask;
        public override Task StartPoint(bool isAsync)                => Task.CompletedTask;
        public override Task SymmetricDifference(bool isAsync)       => Task.CompletedTask;
        public override Task Touches(bool isAsync)                   => Task.CompletedTask;
        public override Task Union(bool isAsync)                     => Task.CompletedTask;
        public override Task Union_void(bool isAsync)                => Task.CompletedTask;
        public override Task Within(bool isAsync)                    => Task.CompletedTask;
        public override Task X(bool isAsync)                         => Task.CompletedTask;
        public override Task Y(bool isAsync)                         => Task.CompletedTask;
        public override Task Z(bool isAsync)                         => Task.CompletedTask;

        #endregion

        void AssertSql(params string[] expected) => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
