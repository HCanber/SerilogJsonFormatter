using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Xunit
{
	public static class ShouldExtensions
	{
		public static string ShouldContain(this string self, string str)
		{
			Assert.Contains(str, self);
			return self;
		}

		public static string ShouldContain(this string self, string str, StringComparison comparison)
		{
			Assert.Contains(str, self, comparison);
			return self;
		}

		public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> self, T item)
		{
			Assert.Contains(item, self);
			return self;
		}

		public static T ShouldContain<T>(this IEnumerable<T> self, Func<T, bool> predicate)
		{
			try
			{
				return self.First(predicate);
			}
			catch(Exception e)
			{
				Assert.True(false, "Expected an item that matched the predicate, but none was found: " + e);
			}

			return default(T);	//Will never get here
		}

		public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> self, T item, IEqualityComparer<T> comparer)
		{
			Assert.Contains(item, self, comparer);
			return self;
		}

		public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> self, params T[] items)
		{
			return ShouldContain(self, EqualityComparer<T>.Default, items);
		}

		public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> self, IEqualityComparer<T> comparer, params T[] items)
		{
			var list = self.ToList();
			foreach(var item in items)
			{
				Assert.Contains(item, list, comparer);
			}
			return list;
		}


		public static IEnumerable<T> ShouldOnlyContain<T>(this IEnumerable<T> self, params T[] items)
		{
			return ShouldOnlyContain(self, EqualityComparer<T>.Default, items);
		}

		public static IEnumerable<T> ShouldOnlyContain<T>(this IEnumerable<T> self, IEqualityComparer<T> comparer, params T[] items)
		{
			var list = self.ToList();
			if(list.Count != items.Length)
			{
				Assert.True(false, string.Format("Expected {0} items. Actual {1}", items.Length, list.Count));
			}
			foreach(var item in items)
			{
				Assert.Contains(item, list, comparer);
			}
			return list;
		}


		public static string ShouldNotContain(this string self, string str)
		{
			Assert.DoesNotContain(str, self);
			return self;
		}

		public static void ShouldNotContain(this string self, string str, StringComparison comparison)
		{
			Assert.DoesNotContain(str, self, comparison);
		}

		public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> self, T item)
		{
			Assert.DoesNotContain(item, self);
			return self;
		}

		public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> self, T item, IEqualityComparer<T> comparer)
		{
			Assert.DoesNotContain(item, self, comparer);
			return self;
		}

		public static IEnumerable ShouldBeEmpty(this IEnumerable self)
		{
			Assert.Empty(self);
			return self;
		}

		public static IEnumerable ShouldNotBeEmpty(this IEnumerable self)
		{
			Assert.NotEmpty(self);
			return self;
		}

		public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> self, int count)
		{
			var actualCount = self.Count();
			Assert.True(actualCount == count, "Expected count to be " + count + ". Actual count: " + actualCount);
			return self;
		}

		public static T ShouldBe<T>(this T self, T other)
		{
			Assert.Equal(other, self);
			return self;
		}

		public static T ShouldBe<T>(this T self, T other, IEqualityComparer<T> comparer)
		{
			Assert.Equal(other, self, comparer);
			return self;
		}

		public static T ShouldNotBe<T>(this T self, T other)
		{
			Assert.NotEqual(other, self);
			return self;
		}

		public static T ShouldNotBe<T>(this T self, T other, IEqualityComparer<T> comparer)
		{
			Assert.NotEqual(other, self, comparer);
			return self;
		}

		public static void ShouldBeNull(this object self)
		{
			Assert.Null(self);
		}

		public static T ShouldNotBeNull<T>(this T self)
		{
			Assert.NotNull(self);
			return self;
		}

		public static T ShouldBeSameAs<T>(this T self, object other)
		{
			Assert.Same(other, self);
			return self;
		}

		public static T ShouldNotBeSameAs<T>(this T self, object other)
		{
			Assert.NotSame(other, self);
			return self;
		}

		public static void ShouldBeTrue(this bool self)
		{
			Assert.True(self);
		}

		public static void ShouldBeTrue(this bool self, string message)
		{
			Assert.True(self, message);
		}

		public static void ShouldBeFalse(this bool self)
		{
			Assert.False(self);
		}

		public static void ShouldBeFalse(this bool self, string message)
		{
			Assert.False(self, message);
		}

		public static T ShouldBeInRange<T>(this T self, T low, T high) where T : IComparable
		{
			Assert.InRange(self, low, high);
			return self;
		}

		public static T ShouldNotBeInRange<T>(this T self, T low, T high) where T : IComparable
		{
			Assert.NotInRange(self, low, high);
			return self;
		}

		public static T ShouldBeGreaterThan<T>(this T self, T other)
				where T : IComparable<T>
		{
			Assert.True(self.CompareTo(other) > 0);
			return self;
		}

		public static T ShouldBeGreaterThan<T>(this T self, T other, IComparer<T> comparer)
		{
			Assert.True(comparer.Compare(self, other) > 0);
			return self;
		}

		public static T ShouldBeGreaterThanOrEqualTo<T>(this T self, T other)
				where T : IComparable<T>
		{
			Assert.True(self.CompareTo(other) >= 0);
			return self;
		}

		public static T ShouldBeGreaterThanOrEqualTo<T>(this T self, T other, IComparer<T> comparer)
		{
			Assert.True(comparer.Compare(self, other) >= 0);
			return self;
		}

		public static T ShouldBeLessThan<T>(this T self, T other)
				where T : IComparable<T>
		{
			Assert.True(self.CompareTo(other) < 0);
			return self;
		}

		public static T ShouldBeLessThan<T>(this T self, T other, IComparer<T> comparer)
		{
			Assert.True(comparer.Compare(self, other) < 0);
			return self;
		}

		public static T ShouldBeLessThanOrEqualTo<T>(this T self, T other)
				where T : IComparable<T>
		{
			Assert.True(self.CompareTo(other) <= 0);
			return self;
		}

		public static T ShouldBeLessThanOrEqualTo<T>(this T self, T other, IComparer<T> comparer)
		{
			Assert.True(comparer.Compare(self, other) <= 0);
			return self;
		}

		public static T ShouldBeInstanceOf<T>(this object self)
		{
			Assert.IsType<T>(self);
			return (T)self;
		}

		public static object ShouldBeInstanceOf(this object self, Type type)
		{
			Assert.IsType(type, self);
			return self;
		}

		public static object ShouldNotBeInstanceOf<T>(this object self)
		{
			Assert.IsNotType<T>(self);
			return self;
		}

		public static object ShouldNotBeInstanceOf(this object self, Type type)
		{
			Assert.IsNotType(type, self);
			return self;
		}

		public static void ShouldThrow<T>(this T self, Action method)
				where T : Exception
		{
			Assert.Throws<T>(() => method());
		}

	}
}
