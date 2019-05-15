/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.digest._apacheCommonsCodec
{

	/// <summary>
	/// Converts String to and from bytes using the encodings required by the Java specification. These encodings are specified in <a
	/// href="http://java.sun.com/j2se/1.4.2/docs/api/java/nio/charset/Charset.html">Standard charsets</a>
	/// </summary>
	/// <seealso cref= <a href="http://commons.apache.org/proper/commons-codec/apidocs/org/apache/commons/codec/CharEncoding.html">CharEncoding</a> </seealso>
	/// <seealso cref= <a href="http://java.sun.com/j2se/1.4.2/docs/api/java/nio/charset/Charset.html">Standard charsets</a>
	/// @author <a href="mailto:ggregory@seagullsw.com">Gary Gregory</a>
	/// @version $Id: StringUtils.java 801391 2009-08-05 19:55:54Z ggregory $
	/// @since 1.4 </seealso>
	public class StringUtils
	{

		public const string UTF_8 = "UTF-8";

		/// <summary>
		/// Constructs a new <code>String</code> by decoding the specified array of bytes using the given charset.
		/// <para>
		/// This method catches <seealso cref="UnsupportedEncodingException"/> and re-throws it as <seealso cref="IllegalStateException"/>, which
		/// should never happen for a required charset name. Use this method when the encoding is required to be in the JRE.
		/// </para>
		/// </summary>
		/// <param name="bytes">
		///            The bytes to be decoded into characters </param>
		/// <param name="charsetName">
		///            The name of a required <seealso cref="java.nio.charset.Charset"/> </param>
		/// <returns> A new <code>String</code> decoded from the specified array of bytes using the given charset. </returns>
		/// <exception cref="IllegalStateException">
		///             Thrown when a <seealso cref="UnsupportedEncodingException"/> is caught, which should never happen for a
		///             required charset name. </exception>
		/// <seealso cref= <a href="http://commons.apache.org/proper/commons-codec/apidocs/org/apache/commons/codec/CharEncoding.html">CharEncoding</a> </seealso>
		/// <seealso cref= String#String(byte[], String) </seealso>
		public static string newString(sbyte[] bytes, string charsetName)
		{
			if (bytes == null)
			{
				return null;
			}
			try
			{
				return StringHelper.NewString(bytes, charsetName);
			}
			catch (UnsupportedEncodingException e)
			{
				throw StringUtils.newIllegalStateException(charsetName, e);
			}
		}

		/// <summary>
		/// Constructs a new <code>String</code> by decoding the specified array of bytes using the UTF-8 charset.
		/// </summary>
		/// <param name="bytes">
		///            The bytes to be decoded into characters </param>
		/// <returns> A new <code>String</code> decoded from the specified array of bytes using the given charset. </returns>
		/// <exception cref="IllegalStateException">
		///             Thrown when a <seealso cref="UnsupportedEncodingException"/> is caught, which should never happen since the
		///             charset is required. </exception>
		public static string newStringUtf8(sbyte[] bytes)
		{
			return StringUtils.newString(bytes, UTF_8);
		}

		/// <summary>
		/// Encodes the given string into a sequence of bytes using the UTF-8 charset, storing the result into a new byte
		/// array.
		/// </summary>
		/// <param name="string">
		///            the String to encode </param>
		/// <returns> encoded bytes </returns>
		/// <exception cref="IllegalStateException">
		///             Thrown when the charset is missing, which should be never according the the Java specification. </exception>
		/// <seealso cref= <a href="http://java.sun.com/j2se/1.4.2/docs/api/java/nio/charset/Charset.html">Standard charsets</a> </seealso>
		/// <seealso cref= #getBytesUnchecked(String, String) </seealso>
		public static sbyte[] getBytesUtf8(string @string)
		{
			return StringUtils.getBytesUnchecked(@string, UTF_8);
		}

		/// <summary>
		/// Encodes the given string into a sequence of bytes using the named charset, storing the result into a new byte
		/// array.
		/// <para>
		/// This method catches <seealso cref="UnsupportedEncodingException"/> and rethrows it as <seealso cref="IllegalStateException"/>, which
		/// should never happen for a required charset name. Use this method when the encoding is required to be in the JRE.
		/// </para>
		/// </summary>
		/// <param name="string">
		///            the String to encode </param>
		/// <param name="charsetName">
		///            The name of a required <seealso cref="java.nio.charset.Charset"/> </param>
		/// <returns> encoded bytes </returns>
		/// <exception cref="IllegalStateException">
		///             Thrown when a <seealso cref="UnsupportedEncodingException"/> is caught, which should never happen for a
		///             required charset name. </exception>
		/// <seealso cref= <a href="http://commons.apache.org/proper/commons-codec/apidocs/org/apache/commons/codec/CharEncoding.html">CharEncoding</a> </seealso>
		/// <seealso cref= String#getBytes(String) </seealso>
		public static sbyte[] getBytesUnchecked(string @string, string charsetName)
		{
			if (string.ReferenceEquals(@string, null))
			{
				return null;
			}
			try
			{
				return @string.GetBytes(charsetName);
			}
			catch (UnsupportedEncodingException e)
			{
				throw StringUtils.newIllegalStateException(charsetName, e);
			}
		}

		private static System.InvalidOperationException newIllegalStateException(string charsetName, UnsupportedEncodingException e)
		{
			return new System.InvalidOperationException(charsetName + ": " + e);
		}
	}

}