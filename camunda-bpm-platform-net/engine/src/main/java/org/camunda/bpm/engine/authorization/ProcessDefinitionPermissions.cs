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
	/// The set of built-in <seealso cref="Permission Permissions"/> for <seealso cref="Resources.PROCESS_DEFINITION Process definition"/> in Camunda BPM.
	/// 
	/// @author Yana Vasileva
	/// 
	/// </summary>
	public sealed class ProcessDefinitionPermissions : Permission
	{

	  /// <summary>
	  /// The none permission means 'no action', 'doing nothing'.
	  /// It does not mean that no permissions are granted. 
	  /// </summary>
	  public static readonly ProcessDefinitionPermissions NONE = new ProcessDefinitionPermissions("NONE", InnerEnum.NONE, "NONE", 0);

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
	  public static readonly ProcessDefinitionPermissions ALL = new ProcessDefinitionPermissions("ALL", InnerEnum.ALL, "ALL", int.MaxValue);

	  /// <summary>
	  /// Indicates that READ interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions READ = new ProcessDefinitionPermissions("READ", InnerEnum.READ, "READ", 2);

	  /// <summary>
	  /// Indicates that UPDATE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions UPDATE = new ProcessDefinitionPermissions("UPDATE", InnerEnum.UPDATE, "UPDATE", 4);

	  /// <summary>
	  /// Indicates that DELETE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions DELETE = new ProcessDefinitionPermissions("DELETE", InnerEnum.DELETE, "DELETE", 16);

	  /// <summary>
	  /// Indicates that READ_TASK interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions READ_TASK = new ProcessDefinitionPermissions("READ_TASK", InnerEnum.READ_TASK, "READ_TASK", 64);

	  /// <summary>
	  /// Indicates that UPDATE_TASK interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions UPDATE_TASK = new ProcessDefinitionPermissions("UPDATE_TASK", InnerEnum.UPDATE_TASK, "UPDATE_TASK", 128);

	  /// <summary>
	  /// Indicates that CREATE_INSTANCE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions CREATE_INSTANCE = new ProcessDefinitionPermissions("CREATE_INSTANCE", InnerEnum.CREATE_INSTANCE, "CREATE_INSTANCE", 256);

	  /// <summary>
	  /// Indicates that READ_INSTANCE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions READ_INSTANCE = new ProcessDefinitionPermissions("READ_INSTANCE", InnerEnum.READ_INSTANCE, "READ_INSTANCE", 512);

	  /// <summary>
	  /// Indicates that UPDATE_INSTANCE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions UPDATE_INSTANCE = new ProcessDefinitionPermissions("UPDATE_INSTANCE", InnerEnum.UPDATE_INSTANCE, "UPDATE_INSTANCE", 1024);

	  /// <summary>
	  /// Indicates that DELETE_INSTANCE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions DELETE_INSTANCE = new ProcessDefinitionPermissions("DELETE_INSTANCE", InnerEnum.DELETE_INSTANCE, "DELETE_INSTANCE", 2048);

	  /// <summary>
	  /// Indicates that READ_HISTORY interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions READ_HISTORY = new ProcessDefinitionPermissions("READ_HISTORY", InnerEnum.READ_HISTORY, "READ_HISTORY", 4096);

	  /// <summary>
	  /// Indicates that DELETE_HISTORY interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions DELETE_HISTORY = new ProcessDefinitionPermissions("DELETE_HISTORY", InnerEnum.DELETE_HISTORY, "DELETE_HISTORY", 8192);

	  /// <summary>
	  /// Indicates that TASK_WORK interactions are permitted </summary>
	  public static readonly ProcessDefinitionPermissions TASK_WORK = new ProcessDefinitionPermissions("TASK_WORK", InnerEnum.TASK_WORK, "TASK_WORK", 16384);

	  /// <summary>
	  /// Indicates that TASK_ASSIGN interactions are permitted </summary>
	  public static readonly ProcessDefinitionPermissions TASK_ASSIGN = new ProcessDefinitionPermissions("TASK_ASSIGN", InnerEnum.TASK_ASSIGN, "TASK_ASSIGN", 32768);

	  /// <summary>
	  /// Indicates that MIGRATE_INSTANCE interactions are permitted </summary>
	  public static readonly ProcessDefinitionPermissions MIGRATE_INSTANCE = new ProcessDefinitionPermissions("MIGRATE_INSTANCE", InnerEnum.MIGRATE_INSTANCE, "MIGRATE_INSTANCE", 65536);

	  /// <summary>
	  /// Indicates that RETRY_JOB interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions RETRY_JOB = new ProcessDefinitionPermissions("RETRY_JOB", InnerEnum.RETRY_JOB, "RETRY_JOB", 32);

	  /// <summary>
	  /// Indicates that SUSPEND interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions SUSPEND = new ProcessDefinitionPermissions("SUSPEND", InnerEnum.SUSPEND, "SUSPEND", 1048576);

	  /// <summary>
	  /// Indicates that SUSPEND_INSTANCE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions SUSPEND_INSTANCE = new ProcessDefinitionPermissions("SUSPEND_INSTANCE", InnerEnum.SUSPEND_INSTANCE, "SUSPEND_INSTANCE", 131072);

	  /// <summary>
	  /// Indicates that UPDATE_INSTANCE_VARIABLE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions UPDATE_INSTANCE_VARIABLE = new ProcessDefinitionPermissions("UPDATE_INSTANCE_VARIABLE", InnerEnum.UPDATE_INSTANCE_VARIABLE, "UPDATE_INSTANCE_VARIABLE", 262144);

	  /// <summary>
	  /// Indicates that UPDATE_TASK_VARIABLE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions UPDATE_TASK_VARIABLE = new ProcessDefinitionPermissions("UPDATE_TASK_VARIABLE", InnerEnum.UPDATE_TASK_VARIABLE, "UPDATE_TASK_VARIABLE", 524288);

	  /// <summary>
	  /// Indicates that READ_INSTANCE_VARIABLE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions READ_INSTANCE_VARIABLE = new ProcessDefinitionPermissions("READ_INSTANCE_VARIABLE", InnerEnum.READ_INSTANCE_VARIABLE, "READ_INSTANCE_VARIABLE", 2097152);

	  /// <summary>
	  /// Indicates that READ_HISTORY_VARIABLE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions READ_HISTORY_VARIABLE = new ProcessDefinitionPermissions("READ_HISTORY_VARIABLE", InnerEnum.READ_HISTORY_VARIABLE, "READ_HISTORY_VARIABLE", 4194304);

	  /// <summary>
	  /// Indicates that READ_TASK_VARIABLE interactions are permitted. </summary>
	  public static readonly ProcessDefinitionPermissions READ_TASK_VARIABLE = new ProcessDefinitionPermissions("READ_TASK_VARIABLE", InnerEnum.READ_TASK_VARIABLE, "READ_TASK_VARIABLE", 8388608);

	  private static readonly IList<ProcessDefinitionPermissions> valueList = new List<ProcessDefinitionPermissions>();

	  static ProcessDefinitionPermissions()
	  {
		  valueList.Add(NONE);
		  valueList.Add(ALL);
		  valueList.Add(READ);
		  valueList.Add(UPDATE);
		  valueList.Add(DELETE);
		  valueList.Add(READ_TASK);
		  valueList.Add(UPDATE_TASK);
		  valueList.Add(CREATE_INSTANCE);
		  valueList.Add(READ_INSTANCE);
		  valueList.Add(UPDATE_INSTANCE);
		  valueList.Add(DELETE_INSTANCE);
		  valueList.Add(READ_HISTORY);
		  valueList.Add(DELETE_HISTORY);
		  valueList.Add(TASK_WORK);
		  valueList.Add(TASK_ASSIGN);
		  valueList.Add(MIGRATE_INSTANCE);
		  valueList.Add(RETRY_JOB);
		  valueList.Add(SUSPEND);
		  valueList.Add(SUSPEND_INSTANCE);
		  valueList.Add(UPDATE_INSTANCE_VARIABLE);
		  valueList.Add(UPDATE_TASK_VARIABLE);
		  valueList.Add(READ_INSTANCE_VARIABLE);
		  valueList.Add(READ_HISTORY_VARIABLE);
		  valueList.Add(READ_TASK_VARIABLE);
	  }

	  public enum InnerEnum
	  {
		  NONE,
		  ALL,
		  READ,
		  UPDATE,
		  DELETE,
		  READ_TASK,
		  UPDATE_TASK,
		  CREATE_INSTANCE,
		  READ_INSTANCE,
		  UPDATE_INSTANCE,
		  DELETE_INSTANCE,
		  READ_HISTORY,
		  DELETE_HISTORY,
		  TASK_WORK,
		  TASK_ASSIGN,
		  MIGRATE_INSTANCE,
		  RETRY_JOB,
		  SUSPEND,
		  SUSPEND_INSTANCE,
		  UPDATE_INSTANCE_VARIABLE,
		  UPDATE_TASK_VARIABLE,
		  READ_INSTANCE_VARIABLE,
		  READ_HISTORY_VARIABLE,
		  READ_TASK_VARIABLE
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private static readonly Resource[] RESOURCES = new Resource[] {Resources.PROCESS_DEFINITION};

	  private string name;
	  private int id;

	  private ProcessDefinitionPermissions(string name, InnerEnum innerEnum, string name, int id)
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


		public static IList<ProcessDefinitionPermissions> values()
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

		public static ProcessDefinitionPermissions valueOf(string name)
		{
			foreach (ProcessDefinitionPermissions enumInstance in ProcessDefinitionPermissions.valueList)
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