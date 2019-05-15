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
	/// The set of built-in <seealso cref="Permission Permissions"/> for <seealso cref="Resources#BATCH Batch operations"/> in Camunda BPM.
	/// 
	/// @author Yana Vasileva
	/// 
	/// </summary>
	public sealed class BatchPermissions : Permission
	{

	  /// <summary>
	  /// The none permission means 'no action', 'doing nothing'.
	  /// It does not mean that no permissions are granted. 
	  /// </summary>
	  public static readonly BatchPermissions NONE = new BatchPermissions("NONE", InnerEnum.NONE, "NONE", 0);

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
	  public static readonly BatchPermissions ALL = new BatchPermissions("ALL", InnerEnum.ALL, "ALL", int.MaxValue);

	  /// <summary>
	  /// Indicates that READ interactions are permitted. </summary>
	  public static readonly BatchPermissions READ = new BatchPermissions("READ", InnerEnum.READ, "READ", 2);

	  /// <summary>
	  /// Indicates that UPDATE interactions are permitted. </summary>
	  public static readonly BatchPermissions UPDATE = new BatchPermissions("UPDATE", InnerEnum.UPDATE, "UPDATE", 4);

	  /// <summary>
	  /// Indicates that CREATE interactions are permitted. </summary>
	  public static readonly BatchPermissions CREATE = new BatchPermissions("CREATE", InnerEnum.CREATE, "CREATE", 8);

	  /// <summary>
	  /// Indicates that DELETE interactions are permitted. </summary>
	  public static readonly BatchPermissions DELETE = new BatchPermissions("DELETE", InnerEnum.DELETE, "DELETE", 16);

	  /// <summary>
	  /// Indicates that READ_HISTORY interactions are permitted. </summary>
	  public static readonly BatchPermissions READ_HISTORY = new BatchPermissions("READ_HISTORY", InnerEnum.READ_HISTORY, "READ_HISTORY", 4096);

	  /// <summary>
	  /// Indicates that DELETE_HISTORY interactions are permitted. </summary>
	  public static readonly BatchPermissions DELETE_HISTORY = new BatchPermissions("DELETE_HISTORY", InnerEnum.DELETE_HISTORY, "DELETE_HISTORY", 8192);

	  // Create Batch specific permissions: //////////////////////

	  /// <summary>
	  /// Indicates that CREATE_BATCH_MIGRATE_PROCESS_INSTANCES interactions are permitted. </summary>
	  public static readonly BatchPermissions CREATE_BATCH_MIGRATE_PROCESS_INSTANCES = new BatchPermissions("CREATE_BATCH_MIGRATE_PROCESS_INSTANCES", InnerEnum.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES, "CREATE_BATCH_MIGRATE_PROCESS_INSTANCES", 32);

	  /// <summary>
	  /// Indicates that CREATE_BATCH_MODIFY_PROCESS_INSTANCES interactions are permitted </summary>
	  public static readonly BatchPermissions CREATE_BATCH_MODIFY_PROCESS_INSTANCES = new BatchPermissions("CREATE_BATCH_MODIFY_PROCESS_INSTANCES", InnerEnum.CREATE_BATCH_MODIFY_PROCESS_INSTANCES, "CREATE_BATCH_MODIFY_PROCESS_INSTANCES", 64);

	  /// <summary>
	  /// Indicates that CREATE_BATCH_RESTART_PROCESS_INSTANCES interactions are permitted </summary>
	  public static readonly BatchPermissions CREATE_BATCH_RESTART_PROCESS_INSTANCES = new BatchPermissions("CREATE_BATCH_RESTART_PROCESS_INSTANCES", InnerEnum.CREATE_BATCH_RESTART_PROCESS_INSTANCES, "CREATE_BATCH_RESTART_PROCESS_INSTANCES", 128);

	  /// <summary>
	  /// Indicates that CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES interactions are permitted </summary>
	  public static readonly BatchPermissions CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES = new BatchPermissions("CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES", InnerEnum.CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES, "CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES", 256);

	  /// <summary>
	  /// Indicates that CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES interactions are permitted. </summary>
	  public static readonly BatchPermissions CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES = new BatchPermissions("CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES", InnerEnum.CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES, "CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES", 512);

	  /// <summary>
	  /// Indicates that CREATE_BATCH_DELETE_DECISION_INSTANCES interactions are permitted </summary>
	  public static readonly BatchPermissions CREATE_BATCH_DELETE_DECISION_INSTANCES = new BatchPermissions("CREATE_BATCH_DELETE_DECISION_INSTANCES", InnerEnum.CREATE_BATCH_DELETE_DECISION_INSTANCES, "CREATE_BATCH_DELETE_DECISION_INSTANCES", 1024);

	  /// <summary>
	  /// Indicates that CREATE_BATCH_SET_JOB_RETRIES interactions are permitted </summary>
	  public static readonly BatchPermissions CREATE_BATCH_SET_JOB_RETRIES = new BatchPermissions("CREATE_BATCH_SET_JOB_RETRIES", InnerEnum.CREATE_BATCH_SET_JOB_RETRIES, "CREATE_BATCH_SET_JOB_RETRIES", 2048);

	  /// <summary>
	  /// Indicates that CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES interactions are permitted </summary>
	  public static readonly BatchPermissions CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES = new BatchPermissions("CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES", InnerEnum.CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES, "CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES", 16384);

	  /// <summary>
	  /// Indicates that CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND interactions are permitted </summary>
	  public static readonly BatchPermissions CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND = new BatchPermissions("CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND", InnerEnum.CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND, "CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND", 32768);

	  /// <summary>
	  /// Indicates that CREATE_BATCH_SET_REMOVAL_TIME interactions are permitted </summary>
	  public static readonly BatchPermissions CREATE_BATCH_SET_REMOVAL_TIME = new BatchPermissions("CREATE_BATCH_SET_REMOVAL_TIME", InnerEnum.CREATE_BATCH_SET_REMOVAL_TIME, "CREATE_BATCH_SET_REMOVAL_TIME", 65536);

	  private static readonly IList<BatchPermissions> valueList = new List<BatchPermissions>();

	  static BatchPermissions()
	  {
		  valueList.Add(NONE);
		  valueList.Add(ALL);
		  valueList.Add(READ);
		  valueList.Add(UPDATE);
		  valueList.Add(CREATE);
		  valueList.Add(DELETE);
		  valueList.Add(READ_HISTORY);
		  valueList.Add(DELETE_HISTORY);
		  valueList.Add(CREATE_BATCH_MIGRATE_PROCESS_INSTANCES);
		  valueList.Add(CREATE_BATCH_MODIFY_PROCESS_INSTANCES);
		  valueList.Add(CREATE_BATCH_RESTART_PROCESS_INSTANCES);
		  valueList.Add(CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES);
		  valueList.Add(CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES);
		  valueList.Add(CREATE_BATCH_DELETE_DECISION_INSTANCES);
		  valueList.Add(CREATE_BATCH_SET_JOB_RETRIES);
		  valueList.Add(CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES);
		  valueList.Add(CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND);
		  valueList.Add(CREATE_BATCH_SET_REMOVAL_TIME);
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
		  DELETE_HISTORY,
		  CREATE_BATCH_MIGRATE_PROCESS_INSTANCES,
		  CREATE_BATCH_MODIFY_PROCESS_INSTANCES,
		  CREATE_BATCH_RESTART_PROCESS_INSTANCES,
		  CREATE_BATCH_DELETE_RUNNING_PROCESS_INSTANCES,
		  CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES,
		  CREATE_BATCH_DELETE_DECISION_INSTANCES,
		  CREATE_BATCH_SET_JOB_RETRIES,
		  CREATE_BATCH_SET_EXTERNAL_TASK_RETRIES,
		  CREATE_BATCH_UPDATE_PROCESS_INSTANCES_SUSPEND,
		  CREATE_BATCH_SET_REMOVAL_TIME
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private static readonly Resource[] RESOURCES = new Resource[] {Resources.BATCH};

	  private string name;
	  private int id;

	  private BatchPermissions(string name, InnerEnum innerEnum, string name, int id)
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

	  public static Permission forName(string name)
	  {
		Permission permission = valueOf(name);
		return permission;
	  }

		public static IList<BatchPermissions> values()
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

		public static BatchPermissions valueOf(string name)
		{
			foreach (BatchPermissions enumInstance in BatchPermissions.valueList)
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