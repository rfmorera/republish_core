﻿using DotNetEssentials;
using DotNetTor.Bases;
using System;

namespace DotNetTor.TorSocks5.Models.Fields.OctetFields
{
	public class VerField : OctetSerializableBase
	{
		#region Statics

		public static VerField Socks5 => new VerField(5);

		#endregion

		#region PropertiesAndMembers

		public int Value => ByteValue;

		#endregion

		#region ConstructorsAndInitializers

		public VerField()
		{
			
		}

		public VerField(int value)
		{
			ByteValue = (byte)Guard.InRangeAndNotNull(nameof(value), value, 0, 255);
		}

		#endregion		
	}
}
