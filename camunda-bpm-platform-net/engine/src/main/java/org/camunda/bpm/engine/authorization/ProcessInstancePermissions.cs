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
namespace org.camunda.bpm.engine.authorization
{
	/// <summary>
	/// The set of built-in <seealso cref="Permission Permissions"/> for <seealso cref="Resources#PROCESS_INSTANCE Process instances"/> in Camunda BPM.
	/// 
	/// @author Yana Vasileva
	/// 
	/// </summary>
	public sealed class ProcessInstancePermissions : Permission
	{

	  /// <summary>
	  /// The none permission means 'no action', 'doing nothing'.
	  /// It does not mean that no permissions are granted. 
	  /// </summary>
	  public static readonly ProcessInstancePermissions NONE = new ProcessInstancePermissions("NONE", InnerEnum.NONE, "NONE", 0);

	  /// <summary>
	  /// Indicates that  all interactions are permitted.
	  /// If ALL is revoked it means that the user is not permitted
	  /// to do everything, which means that at least one permission
	  /// is revoked. This does not implicate that all individual
	  /// permissions are revoked.
	  /// 
	  /// Example: If the UPDATE permission is revoke also the ALL
	  /// permission is revoked, because the user is not authorized
	  /// to execute all actions anymore.
	  /// </summary>
	  public static readonly ProcessInstancePermissions ALL = new ProcessInstancePermissions("ALL", InnerEnum.ALL, "ALL", int.MaxValue);

	  /// <summary>
	  /// Indicates that READ interactions are permitted. </summary>
	  public static readonly ProcessInstancePermissions READ = new ProcessInstancePermissions("READ", InnerEnum.READ, "READ", 2);

	  /// <summary>
	  /// Indicates that UPDATE interactions are permitted. </summary>
	  public static readonly ProcessInstancePermissions UPDATE = new ProcessInstancePermissions("UPDATE", InnerEnum.UPDATE, "UPDATE", 4);

	  /// <summary>
	  /// Indicates that CREATE interactions are permitted. </summary>
	  public static readonly ProcessInstancePermissions CREATE = new ProcessInstancePermissions("CREATE", InnerEnum.CREATE, "CREATE", 8);

	  /// <summary>
	  /// Indicates that DELETE interactions are permitted. </summary>
	  public static readonly ProcessInstancePermissions DELETE = new ProcessInstancePermissions("DELETE", InnerEnum.DELETE, "DELETE", 16);

	  /// <summary>
	  /// Indicates that RETRY_JOB interactions are permitted. </summary>
	  public static readonly ProcessInstancePermissions RETRY_JOB = new ProcessInstancePermissions("RETRY_JOB", InnerEnum.RETRY_JOB, "RETRY_JOB", 32);

	  /// <summary>
	  /// Indicates that SUSPEND interactions are permitted. </summary>
	  public static readonly ProcessInstancePermissions SUSPEND = new ProcessInstancePermissions("SUSPEND", InnerEnum.SUSPEND, "SUSPEND", 64);

	  /// <summary>
	  /// Indicates that UPDATE_VARIABLE interactions are permitted. </summary>
	  public static readonly ProcessInstancePermissions UPDATE_VARIABLE = new ProcessInstancePermissions("UPDATE_VARIABLE", InnerEnum.UPDATE_VARIABLE, "UPDATE_VARIABLE", 128);

	  private static readonly IList<ProcessInstancePermissions> valueList = new List<ProcessInstancePermissions>();

	  static ProcessInstancePermissions()
	  {
		  valueList.Add(NONE);
		  valueList.Add(ALL);
		  valueList.Add(READ);
		  valueList.Add(UPDATE);
		  valueList.Add(CREATE);
		  valueList.Add(DELETE);
		  valueList.Add(RETRY_JOB);
		  valueList.Add(SUSPEND);
		  valueList.Add(UPDATE_VARIABLE);
	  }

	  public enum InnerEnum
	  {
		  NONE,
		  ALL,
		  READ,
		  UPDATE,
		  CREATE,
		  DELETE,
		  RETRY_JOB,
		  SUSPEND,
		  UPDATE_VARIABLE
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private static readonly Resource[] RESOURCES = new Resource[] {Resources.PROCESS_INSTANCE};
	  private string name;
	  private int id;

	  private ProcessInstancePermissions(string name, InnerEnum innerEnum, string name, int id)
	  {
		this.name = name;
		this.id = id;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  public string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public int Value
	  {
		  get
		  {
			return id;
		  }
	  }

	  public Resource[] Types
	  {
		  get
		  {
			return RESOURCES;
		  }
	  }


		public static IList<ProcessInstancePermissions> values()
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

		public static ProcessInstancePermissions valueOf(string name)
		{
			foreach (ProcessInstancePermissions enumInstance in ProcessInstancePermissions.valueList)
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