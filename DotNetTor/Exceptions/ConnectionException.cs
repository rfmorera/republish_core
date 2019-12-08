﻿using System;

namespace DotNetTor.Exceptions
{
	public class ConnectionException : Exception
    {
		public ConnectionException(string message) : base(message)
		{

		}

		public ConnectionException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}
