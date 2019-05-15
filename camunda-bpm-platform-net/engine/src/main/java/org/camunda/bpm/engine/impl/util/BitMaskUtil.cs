﻿/*
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
namespace org.camunda.bpm.engine.impl.util
{


	/// <summary>
	/// Util class for manipulating bit-flag in ints.
	/// 
	/// Currently, only 8-bits are supporten, but can be extended to use all
	/// 31 bits in the integer (1st of 32 bits is used for sign).
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class BitMaskUtil
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  // First 8 masks as constant to prevent having to math.pow() every time a bit needs flippin'.
	  private const int FLAG_BIT_1 = 1; // 000...00000001
	  private const int FLAG_BIT_2 = 2; // 000...00000010
	  private const int FLAG_BIT_3 = 4; // 000...00000100
	  private const int FLAG_BIT_4 = 8; // 000...00001000
	  private const int FLAG_BIT_5 = 16; // 000...00010000
	  private const int FLAG_BIT_6 = 32; // 000...00100000
	  private const int FLAG_BIT_7 = 64; // 000...01000000
	  private const int FLAG_BIT_8 = 128; // 000...10000000

	  private static int[] MASKS = new int[] {FLAG_BIT_1, FLAG_BIT_2, FLAG_BIT_3, FLAG_BIT_4, FLAG_BIT_5, FLAG_BIT_6, FLAG_BIT_7, FLAG_BIT_8};

	  /// <summary>
	  /// Set bit to '1' in the given int. </summary>
	  /// <param name="current"> integer value </param>
	  /// <param name="bitNumber"> number of the bit to set to '1' (right first bit starting at 1). </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static int setBitOn(final int value, final int bitNumber)
	  public static int setBitOn(int value, int bitNumber)
	  {
		ensureBitRange(bitNumber);
		// To turn on, OR with the correct mask
		return value | MASKS[bitNumber - 1];
	  }

	  /// <summary>
	  /// Set bit to '0' in the given int. </summary>
	  /// <param name="current"> integer value </param>
	  /// <param name="bitNumber"> number of the bit to set to '0' (right first bit starting at 1). </param>
	  public static int setBitOff(int value, int bitNumber)
	  {
		ensureBitRange(bitNumber);
		// To turn on, OR with the correct mask
		return value & ~MASKS[bitNumber - 1];
	  }

	  /// <summary>
	  /// Check if the bit is set to '1' </summary>
	  /// <param name="value"> integer to check bit </param>
	  /// <param name="number"> of bit to check (right first bit starting at 1) </param>
	  public static bool isBitOn(int value, int bitNumber)
	  {
		ensureBitRange(bitNumber);
		return ((value & MASKS[bitNumber - 1]) == MASKS[bitNumber - 1]);
	  }

	  /// <summary>
	  /// Set bit to '0' or '1' in the given int. </summary>
	  /// <param name="current"> integer value </param>
	  /// <param name="bitNumber"> number of the bit to set to '0' or '1' (right first bit starting at 1). </param>
	  /// <param name="bitValue"> if true, bit set to '1'. If false, '0'. </param>
	  public static int setBit(int value, int bitNumber, bool bitValue)
	  {
		if (bitValue)
		{
		  return setBitOn(value, bitNumber);
		}
		else
		{
		  return setBitOff(value, bitNumber);
		}
	  }

	  public static int getMaskForBit(int bitNumber)
	  {
		return MASKS[bitNumber - 1];
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static void ensureBitRange(final int bitNumber)
	  private static void ensureBitRange(int bitNumber)
	  {
		if (bitNumber <= 0 && bitNumber > 8)
		{
		  throw LOG.invalidBitNumber(bitNumber);
		}
	  }
	}

}