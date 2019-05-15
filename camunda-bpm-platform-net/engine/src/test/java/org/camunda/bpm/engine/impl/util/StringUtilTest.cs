using System.Text;

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
namespace org.camunda.bpm.engine.impl.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using Test = org.junit.Test;

	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
	public class StringUtilTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTrimToMaximumLengthAllowed()
	  public virtual void testTrimToMaximumLengthAllowed()
	  {
		string fittingThreeByteMessage = repeatCharacter("\u9faf", StringUtil.DB_MAX_STRING_LENGTH);
		string exceedingMessage = repeatCharacter("a", StringUtil.DB_MAX_STRING_LENGTH * 2);

		assertEquals(fittingThreeByteMessage.Substring(0, StringUtil.DB_MAX_STRING_LENGTH), StringUtil.trimToMaximumLengthAllowed(fittingThreeByteMessage));
		assertEquals(exceedingMessage.Substring(0, StringUtil.DB_MAX_STRING_LENGTH), StringUtil.trimToMaximumLengthAllowed(exceedingMessage));
	  }

	  protected internal static string repeatCharacter(string encodedCharacter, int numCharacters)
	  {
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < numCharacters; i++)
		{
		  sb.Append(encodedCharacter);
		}
		return sb.ToString();
	  }
	}

}