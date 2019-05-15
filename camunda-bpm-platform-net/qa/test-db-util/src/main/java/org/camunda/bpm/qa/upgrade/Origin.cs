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
namespace org.camunda.bpm.qa.upgrade
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: There is no attribute target in .NET corresponding to value = {ElementType.TYPE, ElementType.METHOD}:
//ORIGINAL LINE: @Target(value = {ElementType.TYPE, ElementType.METHOD}) @Retention(RetentionPolicy.RUNTIME) public class Origin extends System.Attribute
	[AttributeUsage(<missing>, AllowMultiple = false, Inherited = false)]
	public class Origin : System.Attribute
	{

	  /// <summary>
	  /// The version of the engine that started the scenario
	  /// </summary>
	  internal string value;

		public Origin(String value)
		{
			this.value = value;
		}
	}

}