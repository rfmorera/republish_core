﻿using DotNetEssentials;
using DotNetTor.Bases;
using System;

namespace DotNetTor.TorSocks5.Models.Fields.OctetFields
{
	public class AuthVerField : OctetSerializableBase
	{
		#region Statics

		public static AuthVerField Version1 => new AuthVerField(1);

		#endregion

		#region PropertiesAndMembers

		public int Value => ByteValue;

		#endregion

		#region ConstructorsAndInitializers

		public AuthVerField()
		{

		}

		public AuthVerField(int value)
		{
			ByteValue = (byte)Guard.InRangeAndNotNull(nameof(value), value, 0, 255);
		}

		#endregion
	}
}
