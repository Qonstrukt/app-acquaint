using System;
using System.ComponentModel;

namespace Acquaint.Native.iOS
{
	[Preserve]
	public static class LinkerPreserve
	{
		static LinkerPreserve()
		{
			throw new Exception(new ReferenceConverter(typeof(LinkerPreserve)).ToString());
		}
	}

	public class PreserveAttribute : Attribute
	{
	}
}