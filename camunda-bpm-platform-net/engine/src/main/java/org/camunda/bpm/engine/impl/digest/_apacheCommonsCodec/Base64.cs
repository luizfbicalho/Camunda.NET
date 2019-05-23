using System;
using System.Numerics;

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
	/// Provides Base64 encoding and decoding as defined by RFC 2045.
	/// 
	/// <para>
	/// This class implements section <cite>6.8. Base64 Content-Transfer-Encoding</cite> from RFC 2045 <cite>Multipurpose
	/// Internet Mail Extensions (MIME) Part One: Format of Internet Message Bodies</cite> by Freed and Borenstein.
	/// </para>
	/// <para>
	/// The class can be parameterized in the following manner with various constructors:
	/// <ul>
	/// <li>URL-safe mode: Default off.</li>
	/// <li>Line length: Default 76. Line length that aren't multiples of 4 will still essentially end up being multiples of
	/// 4 in the encoded data.
	/// <li>Line separator: Default is CRLF ("\r\n")</li>
	/// </ul>
	/// </para>
	/// <para>
	/// Since this class operates directly on byte streams, and not character streams, it is hard-coded to only encode/decode
	/// character encodings which are compatible with the lower 127 ASCII chart (ISO-8859-1, Windows-1252, UTF-8, etc).
	/// </para>
	/// </summary>
	/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>
	/// @author Apache Software Foundation
	/// @since 1.0
	/// @version $Id: Base64.java 801706 2009-08-06 16:27:06Z niallp $ </seealso>
	public class Base64
	{
		private const int DEFAULT_BUFFER_RESIZE_FACTOR = 2;

		private const int DEFAULT_BUFFER_SIZE = 8192;

		/// <summary>
		/// Chunk size per RFC 2045 section 6.8.
		/// 
		/// <para>
		/// The {@value} character limit does not count the trailing CRLF, but counts all other characters, including any
		/// equal signs.
		/// </para>
		/// </summary>
		/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045 section 6.8</a> </seealso>
		internal const int CHUNK_SIZE = 76;

		/// <summary>
		/// Chunk separator per RFC 2045 section 2.1.
		/// 
		/// <para>
		/// N.B. The next major release may break compatibility and make this field private.
		/// </para>
		/// </summary>
		/// <seealso cref= <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045 section 2.1</a> </seealso>
		internal static readonly sbyte[] CHUNK_SEPARATOR = new sbyte[] {(sbyte)'\r', (sbyte)'\n'};

		/// <summary>
		/// This array is a lookup table that translates 6-bit positive integer index values into their "Base64 Alphabet"
		/// equivalents as specified in Table 1 of RFC 2045.
		/// 
		/// Thanks to "commons" project in ws.apache.org for this code.
		/// http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
		/// </summary>
		private static readonly sbyte[] STANDARD_ENCODE_TABLE = new sbyte[] {(sbyte)'A', (sbyte)'B', (sbyte)'C', (sbyte)'D', (sbyte)'E', (sbyte)'F', (sbyte)'G', (sbyte)'H', (sbyte)'I', (sbyte)'J', (sbyte)'K', (sbyte)'L', (sbyte)'M', (sbyte)'N', (sbyte)'O', (sbyte)'P', (sbyte)'Q', (sbyte)'R', (sbyte)'S', (sbyte)'T', (sbyte)'U', (sbyte)'V', (sbyte)'W', (sbyte)'X', (sbyte)'Y', (sbyte)'Z', (sbyte)'a', (sbyte)'b', (sbyte)'c', (sbyte)'d', (sbyte)'e', (sbyte)'f', (sbyte)'g', (sbyte)'h', (sbyte)'i', (sbyte)'j', (sbyte)'k', (sbyte)'l', (sbyte)'m', (sbyte)'n', (sbyte)'o', (sbyte)'p', (sbyte)'q', (sbyte)'r', (sbyte)'s', (sbyte)'t', (sbyte)'u', (sbyte)'v', (sbyte)'w', (sbyte)'x', (sbyte)'y', (sbyte)'z', (sbyte)'0', (sbyte)'1', (sbyte)'2', (sbyte)'3', (sbyte)'4', (sbyte)'5', (sbyte)'6', (sbyte)'7', (sbyte)'8', (sbyte)'9', (sbyte)'+', (sbyte)'/'};

		/// <summary>
		/// This is a copy of the STANDARD_ENCODE_TABLE above, but with + and /
		/// changed to - and _ to make the encoded Base64 results more URL-SAFE.
		/// This table is only used when the Base64's mode is set to URL-SAFE.
		/// </summary>
		private static readonly sbyte[] URL_SAFE_ENCODE_TABLE = new sbyte[] {(sbyte)'A', (sbyte)'B', (sbyte)'C', (sbyte)'D', (sbyte)'E', (sbyte)'F', (sbyte)'G', (sbyte)'H', (sbyte)'I', (sbyte)'J', (sbyte)'K', (sbyte)'L', (sbyte)'M', (sbyte)'N', (sbyte)'O', (sbyte)'P', (sbyte)'Q', (sbyte)'R', (sbyte)'S', (sbyte)'T', (sbyte)'U', (sbyte)'V', (sbyte)'W', (sbyte)'X', (sbyte)'Y', (sbyte)'Z', (sbyte)'a', (sbyte)'b', (sbyte)'c', (sbyte)'d', (sbyte)'e', (sbyte)'f', (sbyte)'g', (sbyte)'h', (sbyte)'i', (sbyte)'j', (sbyte)'k', (sbyte)'l', (sbyte)'m', (sbyte)'n', (sbyte)'o', (sbyte)'p', (sbyte)'q', (sbyte)'r', (sbyte)'s', (sbyte)'t', (sbyte)'u', (sbyte)'v', (sbyte)'w', (sbyte)'x', (sbyte)'y', (sbyte)'z', (sbyte)'0', (sbyte)'1', (sbyte)'2', (sbyte)'3', (sbyte)'4', (sbyte)'5', (sbyte)'6', (sbyte)'7', (sbyte)'8', (sbyte)'9', (sbyte)'-', (sbyte)'_'};

		/// <summary>
		/// Byte used to pad output.
		/// </summary>
		private const sbyte PAD = (sbyte)'=';

		/// <summary>
		/// This array is a lookup table that translates Unicode characters drawn from the "Base64 Alphabet" (as specified in
		/// Table 1 of RFC 2045) into their 6-bit positive integer equivalents. Characters that are not in the Base64
		/// alphabet but fall within the bounds of the array are translated to -1.
		/// 
		/// Note: '+' and '-' both decode to 62. '/' and '_' both decode to 63. This means decoder seamlessly handles both
		/// URL_SAFE and STANDARD base64. (The encoder, on the other hand, needs to know ahead of time what to emit).
		/// 
		/// Thanks to "commons" project in ws.apache.org for this code.
		/// http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
		/// </summary>
		private static readonly sbyte[] DECODE_TABLE = new sbyte[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, 62, -1, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, 63, -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51};

		/// <summary>
		/// Mask used to extract 6 bits, used when encoding </summary>
		private const int MASK_6BITS = 0x3f;

		/// <summary>
		/// Mask used to extract 8 bits, used in decoding base64 bytes </summary>
		private const int MASK_8BITS = 0xff;

		// The static final fields above are used for the original static byte[] methods on Base64.
		// The private member fields below are used with the new streaming approach, which requires
		// some state be preserved between calls of encode() and decode().

		/// <summary>
		/// Encode table to use: either STANDARD or URL_SAFE. Note: the DECODE_TABLE above remains static because it is able
		/// to decode both STANDARD and URL_SAFE streams, but the encodeTable must be a member variable so we can switch
		/// between the two modes.
		/// </summary>
		private readonly sbyte[] encodeTable;

		/// <summary>
		/// Line length for encoding. Not used when decoding. A value of zero or less implies no chunking of the base64
		/// encoded data.
		/// </summary>
		private readonly int lineLength;

		/// <summary>
		/// Line separator for encoding. Not used when decoding. Only used if lineLength > 0.
		/// </summary>
		private readonly sbyte[] lineSeparator;

		/// <summary>
		/// Convenience variable to help us determine when our buffer is going to run out of room and needs resizing.
		/// <code>decodeSize = 3 + lineSeparator.length;</code>
		/// </summary>
		private readonly int decodeSize;

		/// <summary>
		/// Convenience variable to help us determine when our buffer is going to run out of room and needs resizing.
		/// <code>encodeSize = 4 + lineSeparator.length;</code>
		/// </summary>
		private readonly int encodeSize;

		/// <summary>
		/// Buffer for streaming.
		/// </summary>
		private sbyte[] buffer;

		/// <summary>
		/// Position where next character should be written in the buffer.
		/// </summary>
		private int pos;

		/// <summary>
		/// Position where next character should be read from the buffer.
		/// </summary>
		private int readPos;

		/// <summary>
		/// Variable tracks how many characters have been written to the current line. Only used when encoding. We use it to
		/// make sure each encoded line never goes beyond lineLength (if lineLength > 0).
		/// </summary>
		private int currentLinePos;

		/// <summary>
		/// Writes to the buffer only occur after every 3 reads when encoding, an every 4 reads when decoding. This variable
		/// helps track that.
		/// </summary>
		private int modulus;

		/// <summary>
		/// Boolean flag to indicate the EOF has been reached. Once EOF has been reached, this Base64 object becomes useless,
		/// and must be thrown away.
		/// </summary>
		private bool eof;

		/// <summary>
		/// Place holder for the 3 bytes we're dealing with for our base64 logic. Bitwise operations store and extract the
		/// base64 encoding or decoding from this variable.
		/// </summary>
		private int x;

		/// <summary>
		/// Creates a Base64 codec used for decoding (all modes) and encoding in URL-unsafe mode.
		/// <para>
		/// When encoding the line length is 76, the line separator is CRLF, and the encoding table is STANDARD_ENCODE_TABLE.
		/// </para>
		/// 
		/// <para>
		/// When decoding all variants are supported.
		/// </para>
		/// </summary>
		public Base64() : this(false)
		{
		}

		/// <summary>
		/// Creates a Base64 codec used for decoding (all modes) and encoding in the given URL-safe mode.
		/// <para>
		/// When encoding the line length is 76, the line separator is CRLF, and the encoding table is STANDARD_ENCODE_TABLE.
		/// </para>
		/// 
		/// <para>
		/// When decoding all variants are supported.
		/// </para>
		/// </summary>
		/// <param name="urlSafe">
		///            if <code>true</code>, URL-safe encoding is used. In most cases this should be set to
		///            <code>false</code>.
		/// @since 1.4 </param>
		public Base64(bool urlSafe) : this(CHUNK_SIZE, CHUNK_SEPARATOR, urlSafe)
		{
		}

		/// <summary>
		/// Creates a Base64 codec used for decoding (all modes) and encoding in URL-unsafe mode.
		/// <para>
		/// When encoding the line length is given in the constructor, the line separator is CRLF, and the encoding table is
		/// STANDARD_ENCODE_TABLE.
		/// </para>
		/// <para>
		/// Line lengths that aren't multiples of 4 will still essentially end up being multiples of 4 in the encoded data.
		/// </para>
		/// <para>
		/// When decoding all variants are supported.
		/// </para>
		/// </summary>
		/// <param name="lineLength">
		///            Each line of encoded data will be at most of the given length (rounded down to nearest multiple of 4).
		///            If lineLength <= 0, then the output will not be divided into lines (chunks). Ignored when decoding.
		/// @since 1.4 </param>
		public Base64(int lineLength) : this(lineLength, CHUNK_SEPARATOR)
		{
		}

		/// <summary>
		/// Creates a Base64 codec used for decoding (all modes) and encoding in URL-unsafe mode.
		/// <para>
		/// When encoding the line length and line separator are given in the constructor, and the encoding table is
		/// STANDARD_ENCODE_TABLE.
		/// </para>
		/// <para>
		/// Line lengths that aren't multiples of 4 will still essentially end up being multiples of 4 in the encoded data.
		/// </para>
		/// <para>
		/// When decoding all variants are supported.
		/// </para>
		/// </summary>
		/// <param name="lineLength">
		///            Each line of encoded data will be at most of the given length (rounded down to nearest multiple of 4).
		///            If lineLength <= 0, then the output will not be divided into lines (chunks). Ignored when decoding. </param>
		/// <param name="lineSeparator">
		///            Each line of encoded data will end with this sequence of bytes. </param>
		/// <exception cref="IllegalArgumentException">
		///             Thrown when the provided lineSeparator included some base64 characters.
		/// @since 1.4 </exception>
		public Base64(int lineLength, sbyte[] lineSeparator) : this(lineLength, lineSeparator, false)
		{
		}

		/// <summary>
		/// Creates a Base64 codec used for decoding (all modes) and encoding in URL-unsafe mode.
		/// <para>
		/// When encoding the line length and line separator are given in the constructor, and the encoding table is
		/// STANDARD_ENCODE_TABLE.
		/// </para>
		/// <para>
		/// Line lengths that aren't multiples of 4 will still essentially end up being multiples of 4 in the encoded data.
		/// </para>
		/// <para>
		/// When decoding all variants are supported.
		/// </para>
		/// </summary>
		/// <param name="lineLength">
		///            Each line of encoded data will be at most of the given length (rounded down to nearest multiple of 4).
		///            If lineLength <= 0, then the output will not be divided into lines (chunks). Ignored when decoding. </param>
		/// <param name="lineSeparator">
		///            Each line of encoded data will end with this sequence of bytes. </param>
		/// <param name="urlSafe">
		///            Instead of emitting '+' and '/' we emit '-' and '_' respectively. urlSafe is only applied to encode
		///            operations. Decoding seamlessly handles both modes. </param>
		/// <exception cref="IllegalArgumentException">
		///             The provided lineSeparator included some base64 characters. That's not going to work!
		/// @since 1.4 </exception>
		public Base64(int lineLength, sbyte[] lineSeparator, bool urlSafe)
		{
			if (lineSeparator == null)
			{
				lineLength = 0; // disable chunk-separating
				lineSeparator = CHUNK_SEPARATOR; // this just gets ignored
			}
			this.lineLength = lineLength > 0 ? (lineLength / 4) * 4 : 0;
			this.lineSeparator = new sbyte[lineSeparator.Length];
			Array.Copy(lineSeparator, 0, this.lineSeparator, 0, lineSeparator.Length);
			if (lineLength > 0)
			{
				this.encodeSize = 4 + lineSeparator.Length;
			}
			else
			{
				this.encodeSize = 4;
			}
			this.decodeSize = this.encodeSize - 1;
			if (containsBase64Byte(lineSeparator))
			{
				string sep = StringUtils.newStringUtf8(lineSeparator);
				throw new System.ArgumentException("lineSeperator must not contain base64 characters: [" + sep + "]");
			}
			this.encodeTable = urlSafe ? URL_SAFE_ENCODE_TABLE : STANDARD_ENCODE_TABLE;
		}

		/// <summary>
		/// Returns our current encode mode. True if we're URL-SAFE, false otherwise.
		/// </summary>
		/// <returns> true if we're in URL-SAFE mode, false otherwise.
		/// @since 1.4 </returns>
		public virtual bool UrlSafe
		{
			get
			{
				return this.encodeTable == URL_SAFE_ENCODE_TABLE;
			}
		}

		/// <summary>
		/// Returns true if this Base64 object has buffered data for reading.
		/// </summary>
		/// <returns> true if there is Base64 object still available for reading. </returns>
		internal virtual bool hasData()
		{
			return this.buffer != null;
		}

		/// <summary>
		/// Returns the amount of buffered data available for reading.
		/// </summary>
		/// <returns> The amount of buffered data available for reading. </returns>
		internal virtual int avail()
		{
			return buffer != null ? pos - readPos : 0;
		}

		/// <summary>
		/// Doubles our buffer. </summary>
		private void resizeBuffer()
		{
			if (buffer == null)
			{
				buffer = new sbyte[DEFAULT_BUFFER_SIZE];
				pos = 0;
				readPos = 0;
			}
			else
			{
				sbyte[] b = new sbyte[buffer.Length * DEFAULT_BUFFER_RESIZE_FACTOR];
				Array.Copy(buffer, 0, b, 0, buffer.Length);
				buffer = b;
			}
		}

		/// <summary>
		/// Extracts buffered data into the provided byte[] array, starting at position bPos, up to a maximum of bAvail
		/// bytes. Returns how many bytes were actually extracted.
		/// </summary>
		/// <param name="b">
		///            byte[] array to extract the buffered data into. </param>
		/// <param name="bPos">
		///            position in byte[] array to start extraction at. </param>
		/// <param name="bAvail">
		///            amount of bytes we're allowed to extract. We may extract fewer (if fewer are available). </param>
		/// <returns> The number of bytes successfully extracted into the provided byte[] array. </returns>
		internal virtual int readResults(sbyte[] b, int bPos, int bAvail)
		{
			if (buffer != null)
			{
				int len = Math.Min(avail(), bAvail);
				if (buffer != b)
				{
					Array.Copy(buffer, readPos, b, bPos, len);
					readPos += len;
					if (readPos >= pos)
					{
						buffer = null;
					}
				}
				else
				{
					// Re-using the original consumer's output array is only
					// allowed for one round.
					buffer = null;
				}
				return len;
			}
			return eof ? -1 : 0;
		}

		/// <summary>
		/// Sets the streaming buffer. This is a small optimization where we try to buffer directly to the consumer's output
		/// array for one round (if the consumer calls this method first) instead of starting our own buffer.
		/// </summary>
		/// <param name="out">
		///            byte[] array to buffer directly to. </param>
		/// <param name="outPos">
		///            Position to start buffering into. </param>
		/// <param name="outAvail">
		///            Amount of bytes available for direct buffering. </param>
		internal virtual void setInitialBuffer(sbyte[] @out, int outPos, int outAvail)
		{
			// We can re-use consumer's original output array under
			// special circumstances, saving on some System.arraycopy().
			if (@out != null && @out.Length == outAvail)
			{
				buffer = @out;
				pos = outPos;
				readPos = outPos;
			}
		}

		/// <summary>
		/// <para>
		/// Encodes all of the provided data, starting at inPos, for inAvail bytes. Must be called at least twice: once with
		/// the data to encode, and once with inAvail set to "-1" to alert encoder that EOF has been reached, so flush last
		/// remaining bytes (if not multiple of 3).
		/// </para>
		/// <para>
		/// Thanks to "commons" project in ws.apache.org for the bitwise operations, and general approach.
		/// http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
		/// </para>
		/// </summary>
		/// <param name="in">
		///            byte[] array of binary data to base64 encode. </param>
		/// <param name="inPos">
		///            Position to start reading data from. </param>
		/// <param name="inAvail">
		///            Amount of bytes available from input for encoding. </param>
		internal virtual void encode(sbyte[] @in, int inPos, int inAvail)
		{
			if (eof)
			{
				return;
			}
			// inAvail < 0 is how we're informed of EOF in the underlying data we're
			// encoding.
			if (inAvail < 0)
			{
				eof = true;
				if (buffer == null || buffer.Length - pos < encodeSize)
				{
					resizeBuffer();
				}
				switch (modulus)
				{
					case 1 :
						buffer[pos++] = encodeTable[(x >> 2) & MASK_6BITS];
						buffer[pos++] = encodeTable[(x << 4) & MASK_6BITS];
						// URL-SAFE skips the padding to further reduce size.
						if (encodeTable == STANDARD_ENCODE_TABLE)
						{
							buffer[pos++] = PAD;
							buffer[pos++] = PAD;
						}
						break;

					case 2 :
						buffer[pos++] = encodeTable[(x >> 10) & MASK_6BITS];
						buffer[pos++] = encodeTable[(x >> 4) & MASK_6BITS];
						buffer[pos++] = encodeTable[(x << 2) & MASK_6BITS];
						// URL-SAFE skips the padding to further reduce size.
						if (encodeTable == STANDARD_ENCODE_TABLE)
						{
							buffer[pos++] = PAD;
						}
						break;
				}
				if (lineLength > 0 && pos > 0)
				{
					Array.Copy(lineSeparator, 0, buffer, pos, lineSeparator.Length);
					pos += lineSeparator.Length;
				}
			}
			else
			{
				for (int i = 0; i < inAvail; i++)
				{
					if (buffer == null || buffer.Length - pos < encodeSize)
					{
						resizeBuffer();
					}
					modulus = (++modulus) % 3;
					int b = @in[inPos++];
					if (b < 0)
					{
						b += 256;
					}
					x = (x << 8) + b;
					if (0 == modulus)
					{
						buffer[pos++] = encodeTable[(x >> 18) & MASK_6BITS];
						buffer[pos++] = encodeTable[(x >> 12) & MASK_6BITS];
						buffer[pos++] = encodeTable[(x >> 6) & MASK_6BITS];
						buffer[pos++] = encodeTable[x & MASK_6BITS];
						currentLinePos += 4;
						if (lineLength > 0 && lineLength <= currentLinePos)
						{
							Array.Copy(lineSeparator, 0, buffer, pos, lineSeparator.Length);
							pos += lineSeparator.Length;
							currentLinePos = 0;
						}
					}
				}
			}
		}

		/// <summary>
		/// <para>
		/// Decodes all of the provided data, starting at inPos, for inAvail bytes. Should be called at least twice: once
		/// with the data to decode, and once with inAvail set to "-1" to alert decoder that EOF has been reached. The "-1"
		/// call is not necessary when decoding, but it doesn't hurt, either.
		/// </para>
		/// <para>
		/// Ignores all non-base64 characters. This is how chunked (e.g. 76 character) data is handled, since CR and LF are
		/// silently ignored, but has implications for other bytes, too. This method subscribes to the garbage-in,
		/// garbage-out philosophy: it will not check the provided data for validity.
		/// </para>
		/// <para>
		/// Thanks to "commons" project in ws.apache.org for the bitwise operations, and general approach.
		/// http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
		/// </para>
		/// </summary>
		/// <param name="in">
		///            byte[] array of ascii data to base64 decode. </param>
		/// <param name="inPos">
		///            Position to start reading data from. </param>
		/// <param name="inAvail">
		///            Amount of bytes available from input for encoding. </param>
		internal virtual void decode(sbyte[] @in, int inPos, int inAvail)
		{
			if (eof)
			{
				return;
			}
			if (inAvail < 0)
			{
				eof = true;
			}
			for (int i = 0; i < inAvail; i++)
			{
				if (buffer == null || buffer.Length - pos < decodeSize)
				{
					resizeBuffer();
				}
				sbyte b = @in[inPos++];
				if (b == PAD)
				{
					// We're done.
					eof = true;
					break;
				}
				else
				{
					if (b >= 0 && b < DECODE_TABLE.Length)
					{
						int result = DECODE_TABLE[b];
						if (result >= 0)
						{
							modulus = (++modulus) % 4;
							x = (x << 6) + result;
							if (modulus == 0)
							{
								buffer[pos++] = (sbyte)((x >> 16) & MASK_8BITS);
								buffer[pos++] = (sbyte)((x >> 8) & MASK_8BITS);
								buffer[pos++] = (sbyte)(x & MASK_8BITS);
							}
						}
					}
				}
			}

			// Two forms of EOF as far as base64 decoder is concerned: actual
			// EOF (-1) and first time '=' character is encountered in stream.
			// This approach makes the '=' padding characters completely optional.
			if (eof && modulus != 0)
			{
				x = x << 6;
				switch (modulus)
				{
					case 2 :
						x = x << 6;
						buffer[pos++] = (sbyte)((x >> 16) & MASK_8BITS);
						break;
					case 3 :
						buffer[pos++] = (sbyte)((x >> 16) & MASK_8BITS);
						buffer[pos++] = (sbyte)((x >> 8) & MASK_8BITS);
						break;
				}
			}
		}

		/// <summary>
		/// Returns whether or not the <code>octet</code> is in the base 64 alphabet.
		/// </summary>
		/// <param name="octet">
		///            The value to test </param>
		/// <returns> <code>true</code> if the value is defined in the the base 64 alphabet, <code>false</code> otherwise.
		/// @since 1.4 </returns>
		public static bool isBase64(sbyte octet)
		{
			return octet == PAD || (octet >= 0 && octet < DECODE_TABLE.Length && DECODE_TABLE[octet] != -1);
		}

		/// <summary>
		/// Tests a given byte array to see if it contains only valid characters within the Base64 alphabet. Currently the
		/// method treats whitespace as valid.
		/// </summary>
		/// <param name="arrayOctet">
		///            byte array to test </param>
		/// <returns> <code>true</code> if all bytes are valid characters in the Base64 alphabet or if the byte array is empty;
		///         false, otherwise </returns>
		public static bool isArrayByteBase64(sbyte[] arrayOctet)
		{
			for (int i = 0; i < arrayOctet.Length; i++)
			{
				if (!isBase64(arrayOctet[i]) && !isWhiteSpace(arrayOctet[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Tests a given byte array to see if it contains only valid characters within the Base64 alphabet.
		/// </summary>
		/// <param name="arrayOctet">
		///            byte array to test </param>
		/// <returns> <code>true</code> if any byte is a valid character in the Base64 alphabet; false herwise </returns>
		private static bool containsBase64Byte(sbyte[] arrayOctet)
		{
			for (int i = 0; i < arrayOctet.Length; i++)
			{
				if (isBase64(arrayOctet[i]))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Encodes binary data using the base64 algorithm but does not chunk the output.
		/// </summary>
		/// <param name="binaryData">
		///            binary data to encode </param>
		/// <returns> byte[] containing Base64 characters in their UTF-8 representation. </returns>
		public static sbyte[] encodeBase64(sbyte[] binaryData)
		{
			return encodeBase64(binaryData, false);
		}

		/// <summary>
		/// Encodes binary data using the base64 algorithm into 76 character blocks separated by CRLF.
		/// </summary>
		/// <param name="binaryData">
		///            binary data to encode </param>
		/// <returns> String containing Base64 characters.
		/// @since 1.4 </returns>
		public static string encodeBase64String(sbyte[] binaryData)
		{
			return StringUtils.newStringUtf8(encodeBase64(binaryData, true));
		}

		/// <summary>
		/// Encodes binary data using a URL-safe variation of the base64 algorithm but does not chunk the output. The
		/// url-safe variation emits - and _ instead of + and / characters.
		/// </summary>
		/// <param name="binaryData">
		///            binary data to encode </param>
		/// <returns> byte[] containing Base64 characters in their UTF-8 representation.
		/// @since 1.4 </returns>
		public static sbyte[] encodeBase64URLSafe(sbyte[] binaryData)
		{
			return encodeBase64(binaryData, false, true);
		}

		/// <summary>
		/// Encodes binary data using a URL-safe variation of the base64 algorithm but does not chunk the output. The
		/// url-safe variation emits - and _ instead of + and / characters.
		/// </summary>
		/// <param name="binaryData">
		///            binary data to encode </param>
		/// <returns> String containing Base64 characters
		/// @since 1.4 </returns>
		public static string encodeBase64URLSafeString(sbyte[] binaryData)
		{
			return StringUtils.newStringUtf8(encodeBase64(binaryData, false, true));
		}

		/// <summary>
		/// Encodes binary data using the base64 algorithm and chunks the encoded output into 76 character blocks
		/// </summary>
		/// <param name="binaryData">
		///            binary data to encode </param>
		/// <returns> Base64 characters chunked in 76 character blocks </returns>
		public static sbyte[] encodeBase64Chunked(sbyte[] binaryData)
		{
			return encodeBase64(binaryData, true);
		}

		/// <summary>
		/// Decodes an Object using the base64 algorithm. This method is provided in order to satisfy the requirements of the
		/// Decoder interface, and will throw a DecoderException if the supplied object is not of type byte[] or String.
		/// </summary>
		/// <param name="pObject">
		///            Object to decode </param>
		/// <returns> An object (of type byte[]) containing the binary data which corresponds to the byte[] or String supplied. </returns>
		/// <exception cref="DecoderException">
		///             if the parameter supplied is not of type byte[] </exception>
		public virtual object decode(object pObject)
		{
			if (pObject is sbyte[])
			{
				return decode((sbyte[]) pObject);
			}
			else if (pObject is string)
			{
				return decode((string) pObject);
			}
			else
			{
				throw new Exception("Parameter supplied to Base64 decode is not a byte[] or a String");
			}
		}

		/// <summary>
		/// Decodes a String containing containing characters in the Base64 alphabet.
		/// </summary>
		/// <param name="pArray">
		///            A String containing Base64 character data </param>
		/// <returns> a byte array containing binary data
		/// @since 1.4 </returns>
		public virtual sbyte[] decode(string pArray)
		{
			return decode(StringUtils.getBytesUtf8(pArray));
		}

		/// <summary>
		/// Decodes a byte[] containing containing characters in the Base64 alphabet.
		/// </summary>
		/// <param name="pArray">
		///            A byte array containing Base64 character data </param>
		/// <returns> a byte array containing binary data </returns>
		public virtual sbyte[] decode(sbyte[] pArray)
		{
			reset();
			if (pArray == null || pArray.Length == 0)
			{
				return pArray;
			}
			long len = (pArray.Length * 3) / 4;
			sbyte[] buf = new sbyte[(int) len];
			setInitialBuffer(buf, 0, buf.Length);
			decode(pArray, 0, pArray.Length);
			decode(pArray, 0, -1); // Notify decoder of EOF.

			// Would be nice to just return buf (like we sometimes do in the encode
			// logic), but we have no idea what the line-length was (could even be
			// variable).  So we cannot determine ahead of time exactly how big an
			// array is necessary.  Hence the need to construct a 2nd byte array to
			// hold the final result:

			sbyte[] result = new sbyte[pos];
			readResults(result, 0, result.Length);
			return result;
		}

		/// <summary>
		/// Encodes binary data using the base64 algorithm, optionally chunking the output into 76 character blocks.
		/// </summary>
		/// <param name="binaryData">
		///            Array containing binary data to encode. </param>
		/// <param name="isChunked">
		///            if <code>true</code> this encoder will chunk the base64 output into 76 character blocks </param>
		/// <returns> Base64-encoded data. </returns>
		/// <exception cref="IllegalArgumentException">
		///             Thrown when the input array needs an output array bigger than <seealso cref="Integer.MAX_VALUE"/> </exception>
		public static sbyte[] encodeBase64(sbyte[] binaryData, bool isChunked)
		{
			return encodeBase64(binaryData, isChunked, false);
		}

		/// <summary>
		/// Encodes binary data using the base64 algorithm, optionally chunking the output into 76 character blocks.
		/// </summary>
		/// <param name="binaryData">
		///            Array containing binary data to encode. </param>
		/// <param name="isChunked">
		///            if <code>true</code> this encoder will chunk the base64 output into 76 character blocks </param>
		/// <param name="urlSafe">
		///            if <code>true</code> this encoder will emit - and _ instead of the usual + and / characters. </param>
		/// <returns> Base64-encoded data. </returns>
		/// <exception cref="IllegalArgumentException">
		///             Thrown when the input array needs an output array bigger than <seealso cref="Integer.MAX_VALUE"/>
		/// @since 1.4 </exception>
		public static sbyte[] encodeBase64(sbyte[] binaryData, bool isChunked, bool urlSafe)
		{
			return encodeBase64(binaryData, isChunked, urlSafe, int.MaxValue);
		}

		/// <summary>
		/// Encodes binary data using the base64 algorithm, optionally chunking the output into 76 character blocks.
		/// </summary>
		/// <param name="binaryData">
		///            Array containing binary data to encode. </param>
		/// <param name="isChunked">
		///            if <code>true</code> this encoder will chunk the base64 output into 76 character blocks </param>
		/// <param name="urlSafe">
		///            if <code>true</code> this encoder will emit - and _ instead of the usual + and / characters. </param>
		/// <param name="maxResultSize">
		///            The maximum result size to accept. </param>
		/// <returns> Base64-encoded data. </returns>
		/// <exception cref="IllegalArgumentException">
		///             Thrown when the input array needs an output array bigger than maxResultSize
		/// @since 1.4 </exception>
		public static sbyte[] encodeBase64(sbyte[] binaryData, bool isChunked, bool urlSafe, int maxResultSize)
		{
			if (binaryData == null || binaryData.Length == 0)
			{
				return binaryData;
			}

			long len = getEncodeLength(binaryData, CHUNK_SIZE, CHUNK_SEPARATOR);
			if (len > maxResultSize)
			{
				throw new System.ArgumentException("Input array too big, the output array would be bigger (" + len + ") than the specified maxium size of " + maxResultSize);
			}

			Base64 b64 = isChunked ? new Base64(urlSafe) : new Base64(0, CHUNK_SEPARATOR, urlSafe);
			return b64.encode(binaryData);
		}

		/// <summary>
		/// Decodes a Base64 String into octets
		/// </summary>
		/// <param name="base64String">
		///            String containing Base64 data </param>
		/// <returns> Array containing decoded data.
		/// @since 1.4 </returns>
		public static sbyte[] decodeBase64(string base64String)
		{
			return (new Base64()).decode(base64String);
		}

		/// <summary>
		/// Decodes Base64 data into octets
		/// </summary>
		/// <param name="base64Data">
		///            Byte array containing Base64 data </param>
		/// <returns> Array containing decoded data. </returns>
		public static sbyte[] decodeBase64(sbyte[] base64Data)
		{
			return (new Base64()).decode(base64Data);
		}

		/// <summary>
		/// Discards any whitespace from a base-64 encoded block.
		/// </summary>
		/// <param name="data">
		///            The base-64 encoded data to discard the whitespace from. </param>
		/// <returns> The data, less whitespace (see RFC 2045). </returns>
		/// @deprecated This method is no longer needed 
		internal static sbyte[] discardWhitespace(sbyte[] data)
		{
			sbyte[] groomedData = new sbyte[data.Length];
			int bytesCopied = 0;
			for (int i = 0; i < data.Length; i++)
			{
				switch (data[i])
				{
					case ' ' :
					case '\n' :
					case '\r' :
					case '\t' :
						break;
					default :
						groomedData[bytesCopied++] = data[i];
					break;
				}
			}
			sbyte[] packedData = new sbyte[bytesCopied];
			Array.Copy(groomedData, 0, packedData, 0, bytesCopied);
			return packedData;
		}

		/// <summary>
		/// Checks if a byte value is whitespace or not.
		/// </summary>
		/// <param name="byteToCheck">
		///            the byte to check </param>
		/// <returns> true if byte is whitespace, false otherwise </returns>
		private static bool isWhiteSpace(sbyte byteToCheck)
		{
			switch (byteToCheck)
			{
				case ' ' :
				case '\n' :
				case '\r' :
				case '\t' :
					return true;
				default :
					return false;
			}
		}

		// Implementation of the Encoder Interface

		/// <summary>
		/// Encodes an Object using the base64 algorithm. This method is provided in order to satisfy the requirements of the
		/// Encoder interface, and will throw an EncoderException if the supplied object is not of type byte[].
		/// </summary>
		/// <param name="pObject">
		///            Object to encode </param>
		/// <returns> An object (of type byte[]) containing the base64 encoded data which corresponds to the byte[] supplied. </returns>
		/// <exception cref="EncoderException">
		///             if the parameter supplied is not of type byte[] </exception>
		public virtual object encode(object pObject)
		{
			if (!(pObject is sbyte[]))
			{
				throw new Exception("Parameter supplied to Base64 encode is not a byte[]");
			}
			return encode((sbyte[]) pObject);
		}

		/// <summary>
		/// Encodes a byte[] containing binary data, into a String containing characters in the Base64 alphabet.
		/// </summary>
		/// <param name="pArray">
		///            a byte array containing binary data </param>
		/// <returns> A String containing only Base64 character data
		/// @since 1.4 </returns>
		public virtual string encodeToString(sbyte[] pArray)
		{
			return StringUtils.newStringUtf8(encode(pArray));
		}

		/// <summary>
		/// Encodes a byte[] containing binary data, into a byte[] containing characters in the Base64 alphabet.
		/// </summary>
		/// <param name="pArray">
		///            a byte array containing binary data </param>
		/// <returns> A byte array containing only Base64 character data </returns>
		public virtual sbyte[] encode(sbyte[] pArray)
		{
			reset();
			if (pArray == null || pArray.Length == 0)
			{
				return pArray;
			}
			long len = getEncodeLength(pArray, lineLength, lineSeparator);
			sbyte[] buf = new sbyte[(int) len];
			setInitialBuffer(buf, 0, buf.Length);
			encode(pArray, 0, pArray.Length);
			encode(pArray, 0, -1); // Notify encoder of EOF.
			// Encoder might have resized, even though it was unnecessary.
			if (buffer != buf)
			{
				readResults(buf, 0, buf.Length);
			}
			// In URL-SAFE mode we skip the padding characters, so sometimes our
			// final length is a bit smaller.
			if (UrlSafe && pos < buf.Length)
			{
				sbyte[] smallerBuf = new sbyte[pos];
				Array.Copy(buf, 0, smallerBuf, 0, pos);
				buf = smallerBuf;
			}
			return buf;
		}

		/// <summary>
		/// Pre-calculates the amount of space needed to base64-encode the supplied array.
		/// </summary>
		/// <param name="pArray"> byte[] array which will later be encoded </param>
		/// <param name="chunkSize"> line-length of the output (<= 0 means no chunking) between each
		///        chunkSeparator (e.g. CRLF). </param>
		/// <param name="chunkSeparator"> the sequence of bytes used to separate chunks of output (e.g. CRLF).
		/// </param>
		/// <returns> amount of space needed to encoded the supplied array.  Returns
		///         a long since a max-len array will require Integer.MAX_VALUE + 33%. </returns>
		private static long getEncodeLength(sbyte[] pArray, int chunkSize, sbyte[] chunkSeparator)
		{
			// base64 always encodes to multiples of 4.
			chunkSize = (chunkSize / 4) * 4;

			long len = (pArray.Length * 4) / 3;
			long mod = len % 4;
			if (mod != 0)
			{
				len += 4 - mod;
			}
			if (chunkSize > 0)
			{
				bool lenChunksPerfectly = len % chunkSize == 0;
				len += (len / chunkSize) * chunkSeparator.Length;
				if (!lenChunksPerfectly)
				{
					len += chunkSeparator.Length;
				}
			}
			return len;
		}

		// Implementation of integer encoding used for crypto
		/// <summary>
		/// Decodes a byte64-encoded integer according to crypto standards such as W3C's XML-Signature
		/// </summary>
		/// <param name="pArray">
		///            a byte array containing base64 character data </param>
		/// <returns> A BigInteger
		/// @since 1.4 </returns>
		public static BigInteger decodeInteger(sbyte[] pArray)
		{
			return new BigInteger(1, decodeBase64(pArray));
		}

		/// <summary>
		/// Encodes to a byte64-encoded integer according to crypto standards such as W3C's XML-Signature
		/// </summary>
		/// <param name="bigInt">
		///            a BigInteger </param>
		/// <returns> A byte array containing base64 character data </returns>
		/// <exception cref="NullPointerException">
		///             if null is passed in
		/// @since 1.4 </exception>
		public static sbyte[] encodeInteger(BigInteger bigInt)
		{
			if (bigInt == null)
			{
				throw new System.NullReferenceException("encodeInteger called with null parameter");
			}
			return encodeBase64(toIntegerBytes(bigInt), false);
		}

		/// <summary>
		/// Returns a byte-array representation of a <code>BigInteger</code> without sign bit.
		/// </summary>
		/// <param name="bigInt">
		///            <code>BigInteger</code> to be converted </param>
		/// <returns> a byte array representation of the BigInteger parameter </returns>
		internal static sbyte[] toIntegerBytes(BigInteger bigInt)
		{
			int bitlen = bigInt.bitLength();
			// round bitlen
			bitlen = ((bitlen + 7) >> 3) << 3;
			sbyte[] bigBytes = bigInt.ToByteArray();

			if (((bigInt.bitLength() % 8) != 0) && (((bigInt.bitLength() / 8) + 1) == (bitlen / 8)))
			{
				return bigBytes;
			}
			// set up params for copying everything but sign bit
			int startSrc = 0;
			int len = bigBytes.Length;

			// if bigInt is exactly byte-aligned, just skip signbit in copy
			if ((bigInt.bitLength() % 8) == 0)
			{
				startSrc = 1;
				len--;
			}
			int startDst = bitlen / 8 - len; // to pad w/ nulls as per spec
			sbyte[] resizedBytes = new sbyte[bitlen / 8];
			Array.Copy(bigBytes, startSrc, resizedBytes, startDst, len);
			return resizedBytes;
		}

		/// <summary>
		/// Resets this Base64 object to its initial newly constructed state.
		/// </summary>
		private void reset()
		{
			buffer = null;
			pos = 0;
			readPos = 0;
			currentLinePos = 0;
			modulus = 0;
			eof = false;
		}

	}

}