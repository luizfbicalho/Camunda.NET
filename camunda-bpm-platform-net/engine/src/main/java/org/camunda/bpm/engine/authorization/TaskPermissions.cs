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
	/// The set of built-in <seealso cref="Permission Permissions"/> for <seealso cref="Resources.TASK Task"/> in Camunda BPM.
	/// 
	/// @author Yana Vasileva
	/// 
	/// </summary>
	public sealed class TaskPermissions : Permission
	{

	  /// <summary>
	  /// The none permission means 'no action', 'doing nothing'.
	  /// It does not mean that no permissions are granted. 
	  /// </summary>
	  public static readonly TaskPermissions NONE = new TaskPermissions("NONE", InnerEnum.NONE, "NONE", 0);

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
	  public static readonly TaskPermissions ALL = new TaskPermissions("ALL", InnerEnum.ALL, "ALL", int.MaxValue);

	  /// <summary>
	  /// Indicates that READ interactions are permitted. </summary>
	  public static readonly TaskPermissions READ = new TaskPermissions("READ", InnerEnum.READ, "READ", 2);

	  /// <summary>
	  /// Indicates that UPDATE interactions are permitted. </summary>
	  public static readonly TaskPermissions UPDATE = new TaskPermissions("UPDATE", InnerEnum.UPDATE, "UPDATE", 4);

	  /// <summary>
	  /// Indicates that CREATE interactions are permitted. </summary>
	  public static readonly TaskPermissions CREATE = new TaskPermissions("CREATE", InnerEnum.CREATE, "CREATE", 8);

	  /// <summary>
	  /// Indicates that DELETE interactions are permitted. </summary>
	  public static readonly TaskPermissions DELETE = new TaskPermissions("DELETE", InnerEnum.DELETE, "DELETE", 16);

	  /// <summary>
	  /// Indicates that READ_HISTORY interactions are permitted. </summary>
	  public static readonly TaskPermissions READ_HISTORY = new TaskPermissions("READ_HISTORY", InnerEnum.READ_HISTORY, "READ_HISTORY", 4096);

	  /// <summary>
	  /// Indicates that TASK_WORK interactions are permitted </summary>
	  public static readonly TaskPermissions TASK_WORK = new TaskPermissions("TASK_WORK", InnerEnum.TASK_WORK, "TASK_WORK", 16384);

	  /// <summary>
	  /// Indicates that TASK_ASSIGN interactions are permitted </summary>
	  public static readonly TaskPermissions TASK_ASSIGN = new TaskPermissions("TASK_ASSIGN", InnerEnum.TASK_ASSIGN, "TASK_ASSIGN", 32768);

	  /// <summary>
	  /// Indicates that UPDATE_VARIABLE interactions are permitted. </summary>
	  public static readonly TaskPermissions UPDATE_VARIABLE = new TaskPermissions("UPDATE_VARIABLE", InnerEnum.UPDATE_VARIABLE, "UPDATE_VARIABLE", 32);

	  /// <summary>
	  /// Indicates that READ_VARIABLE interactions are permitted. </summary>
	  public static readonly TaskPermissions READ_VARIABLE = new TaskPermissions("READ_VARIABLE", InnerEnum.READ_VARIABLE, "READ_VARIABLE", 64);

	  private static readonly IList<TaskPermissions> valueList = new List<TaskPermissions>();

	  static TaskPermissions()
	  {
		  valueList.Add(NONE);
		  valueList.Add(ALL);
		  valueList.Add(READ);
		  valueList.Add(UPDATE);
		  valueList.Add(CREATE);
		  valueList.Add(DELETE);
		  valueList.Add(READ_HISTORY);
		  valueList.Add(TASK_WORK);
		  valueList.Add(TASK_ASSIGN);
		  valueList.Add(UPDATE_VARIABLE);
		  valueList.Add(READ_VARIABLE);
	  }

	  public enum InnerEnum
	  {
		  NONE,
		  ALL,
		  READ,
		  UPDATE,
		  CREATE,
		  DELETE,
		  READ_HISTORY,
		  TASK_WORK,
		  TASK_ASSIGN,
		  UPDATE_VARIABLE,
		  READ_VARIABLE
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private static readonly Resource[] RESOURCES = new Resource[] {Resources.TASK};

	  private string name;
	  private int id;

	  private TaskPermissions(string name, InnerEnum innerEnum, string name, int id)
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


		public static IList<TaskPermissions> values()
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

		public static TaskPermissions valueOf(string name)
		{
			foreach (TaskPermissions enumInstance in TaskPermissions.valueList)
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