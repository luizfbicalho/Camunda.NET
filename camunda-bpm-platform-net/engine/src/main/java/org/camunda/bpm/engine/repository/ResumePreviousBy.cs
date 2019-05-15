using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.repository
{

	/// <summary>
	/// Contains the constants for the possible values the property <seealso cref="ProcessApplicationDeploymentBuilder#resumePreviousVersionsBy(String)"/>.
	/// </summary>
	public sealed class ResumePreviousBy
	{
	  public static readonly ResumePreviousBy  = new ResumePreviousBy("", InnerEnum.);

	  private static readonly IList<ResumePreviousBy> valueList = new List<ResumePreviousBy>();

	  static ResumePreviousBy()
	  {
		  valueList.Add();
	  }

	  public enum InnerEnum
	  {
          
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private ResumePreviousBy(string name, InnerEnum innerEnum)
	  {
		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  /// <summary>
	  /// Resume previous deployments that contain processes with the same key as in the new deployment
	  /// </summary>
	  public const string RESUME_BY_PROCESS_DEFINITION_KEY = "process-definition-key";

	  /// <summary>
	  /// Resume previous deployments that have the same name as the new deployment
	  /// </summary>
	  public const string RESUME_BY_DEPLOYMENT_NAME = "deployment-name";

		public static IList<ResumePreviousBy> values()
		{
			return valueList;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static ResumePreviousBy valueOf(string name)
		{
			foreach (ResumePreviousBy enumInstance in ResumePreviousBy.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}
}