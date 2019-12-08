﻿using DotNetTor.Bases;
using DotNetTor.TorSocks5.Models.Fields.OctetFields;
using System;
using System.Linq;
using DotNetTor.TorSocks5.Models.TorSocks5.Fields.ByteArrayFields;
using DotNetEssentials;

namespace DotNetTor.TorSocks5.Models.Messages
{
	public class VersionMethodRequest : ByteArraySerializableBase
    {
		#region PropertiesAndMembers

		public VerField Ver { get; set; }

		public NMethodsField NMethods { get; set; }

		public MethodsField Methods { get; set; }

		#endregion

		#region ConstructorsAndInitializers

		public VersionMethodRequest()
		{

		}

		public VersionMethodRequest(MethodsField methods)
		{
			Methods = Guard.NotNull(nameof(methods), methods);

			Ver = VerField.Socks5;

			// The NMETHODS field contains the number of method identifier octets that appear in the METHODS field.
			var nMethods = new NMethodsField();
			nMethods.FromMethodsField(methods);
			NMethods = nMethods;
		}

		#endregion

		#region Serialization

		public override void FromBytes(byte[] bytes)
		{
			Guard.NotNullOrEmpty(nameof(bytes), bytes);
			Guard.InRangeAndNotNull($"{nameof(bytes)}.{nameof(bytes.Length)}", bytes.Length, 3, 257);

			Ver = new VerField();
			Ver.FromByte(bytes[0]);

			NMethods = new NMethodsField();
			NMethods.FromByte(bytes[1]);

			if(NMethods.Value != bytes.Length - 2)
			{
				throw new FormatException($"{nameof(NMethods)}.{nameof(NMethods.Value)} must be {nameof(bytes)}.{nameof(bytes.Length)} - 2` = {bytes.Length - 2}. Actual: {NMethods.Value}.");
			}

			Methods = new MethodsField();
			Methods.FromBytes(bytes.Skip(2).ToArray());
		}

		public override byte[] ToBytes() => ByteHelpers.Combine(new byte[] { Ver.ToByte(), NMethods.ToByte() }, Methods.ToBytes());

		#endregion
	}
}
