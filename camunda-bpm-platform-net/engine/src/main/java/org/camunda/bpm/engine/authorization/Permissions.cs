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
	/// The set of built-in <seealso cref="Permission Permissions"/> for camunda BPM.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public sealed class Permissions : Permission
	{

	  /// <summary>
	  /// The none permission means 'no action', 'doing nothing'.
	  /// It does not mean that no permissions are granted. 
	  /// </summary>
	  public static readonly Permissions NONE = new Permissions("NONE", InnerEnum.NONE, "NONE", 0, java.util.EnumSet.allOf(typeof(Resources)));

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
	  public static readonly Permissions ALL = new Permissions("ALL", InnerEnum.ALL, "ALL", int.MaxValue, java.util.EnumSet.allOf(typeof(Resources)));

	  /// <summary>
	  /// Indicates that READ interactions are permitted. </summary>
	  public static readonly Permissions READ = new Permissions("READ", InnerEnum.READ, "READ", 2, java.util.EnumSet.of(Resources.AUTHORIZATION, Resources.BATCH, Resources.DASHBOARD, Resources.DECISION_DEFINITION, Resources.DECISION_REQUIREMENTS_DEFINITION, Resources.DEPLOYMENT, Resources.FILTER, Resources.GROUP, Resources.PROCESS_DEFINITION, Resources.PROCESS_INSTANCE, Resources.REPORT, Resources.TASK, Resources.TENANT, Resources.USER));

	  /// <summary>
	  /// Indicates that UPDATE interactions are permitted. </summary>
	  public static readonly Permissions UPDATE = new Permissions("UPDATE", InnerEnum.UPDATE, "UPDATE", 4, java.util.EnumSet.of(Resources.AUTHORIZATION, Resources.BATCH, Resources.DASHBOARD, Resources.DECISION_DEFINITION, Resources.FILTER, Resources.GROUP, Resources.PROCESS_DEFINITION, Resources.PROCESS_INSTANCE, Resources.REPORT, Resources.TASK, Resources.TENANT, Resources.USER));

	  /// <summary>
	  /// Indicates that CREATE interactions are permitted. </summary>
	  public static readonly Permissions CREATE = new Permissions("CREATE", InnerEnum.CREATE, "CREATE", 8, java.util.EnumSet.of(Resources.AUTHORIZATION, Resources.BATCH, Resources.DASHBOARD, Resources.DEPLOYMENT, Resources.FILTER, Resources.GROUP, Resources.GROUP_MEMBERSHIP, Resources.PROCESS_INSTANCE, Resources.REPORT, Resources.TASK, Resources.TENANT, Resources.TENANT_MEMBERSHIP, Resources.USER));

	  /// <summary>
	  /// Indicates that DELETE interactions are permitted. </summary>
	  public static readonly Permissions DELETE = new Permissions("DELETE", InnerEnum.DELETE, "DELETE", 16, java.util.EnumSet.of(Resources.AUTHORIZATION, Resources.BATCH, Resources.DASHBOARD, Resources.DEPLOYMENT, Resources.FILTER, Resources.GROUP, Resources.GROUP_MEMBERSHIP, Resources.PROCESS_DEFINITION, Resources.PROCESS_INSTANCE, Resources.REPORT, Resources.TASK, Resources.TENANT, Resources.TENANT_MEMBERSHIP, Resources.USER));

	  /// <summary>
	  /// Indicates that ACCESS interactions are permitted. </summary>
	  public static readonly Permissions ACCESS = new Permissions("ACCESS", InnerEnum.ACCESS, "ACCESS", 32, java.util.EnumSet.of(Resources.APPLICATION));

	  /// <summary>
	  /// Indicates that READ_TASK interactions are permitted. </summary>
	  public static readonly Permissions READ_TASK = new Permissions("READ_TASK", InnerEnum.READ_TASK, "READ_TASK", 64, java.util.EnumSet.of(Resources.PROCESS_DEFINITION));

	  /// <summary>
	  /// Indicates that UPDATE_TASK interactions are permitted. </summary>
	  public static readonly Permissions UPDATE_TASK = new Permissions("UPDATE_TASK", InnerEnum.UPDATE_TASK, "UPDATE_TASK", 128, java.util.EnumSet.of(Resources.PROCESS_DEFINITION));

	  /// <summary>
	  /// Indicates that CREATE_INSTANCE interactions are permitted. </summary>
	  public static readonly Permissions CREATE_INSTANCE = new Permissions("CREATE_INSTANCE", InnerEnum.CREATE_INSTANCE, "CREATE_INSTANCE", 256, java.util.EnumSet.of(Resources.DECISION_DEFINITION, Resources.PROCESS_DEFINITION));

	  /// <summary>
	  /// Indicates that READ_INSTANCE interactions are permitted. </summary>
	  public static readonly Permissions READ_INSTANCE = new Permissions("READ_INSTANCE", InnerEnum.READ_INSTANCE, "READ_INSTANCE", 512, java.util.EnumSet.of(Resources.PROCESS_DEFINITION));

	  /// <summary>
	  /// Indicates that UPDATE_INSTANCE interactions are permitted. </summary>
	  public static readonly Permissions UPDATE_INSTANCE = new Permissions("UPDATE_INSTANCE", InnerEnum.UPDATE_INSTANCE, "UPDATE_INSTANCE", 1024, java.util.EnumSet.of(Resources.PROCESS_DEFINITION));

	  /// <summary>
	  /// Indicates that DELETE_INSTANCE interactions are permitted. </summary>
	  public static readonly Permissions DELETE_INSTANCE = new Permissions("DELETE_INSTANCE", InnerEnum.DELETE_INSTANCE, "DELETE_INSTANCE", 2048, java.util.EnumSet.of(Resources.PROCESS_DEFINITION));

	  /// <summary>
	  /// Indicates that READ_HISTORY interactions are permitted. </summary>
	  public static readonly Permissions READ_HISTORY = new Permissions("READ_HISTORY", InnerEnum.READ_HISTORY, "READ_HISTORY", 4096, java.util.EnumSet.of(Resources.BATCH, Resources.DECISION_DEFINITION, Resources.PROCESS_DEFINITION, Resources.TASK));

	  /// <summary>
	  /// Indicates that DELETE_HISTORY interactions are permitted. </summary>
	  public static readonly Permissions DELETE_HISTORY = new Permissions("DELETE_HISTORY", InnerEnum.DELETE_HISTORY, "DELETE_HISTORY", 8192, java.util.EnumSet.of(Resources.BATCH, Resources.DECISION_DEFINITION, Resources.PROCESS_DEFINITION));

	  /// <summary>
	  /// Indicates that TASK_WORK interactions are permitted </summary>
	  public static readonly Permissions TASK_WORK = new Permissions("TASK_WORK", InnerEnum.TASK_WORK, "TASK_WORK", 16384, java.util.EnumSet.of(Resources.PROCESS_DEFINITION, Resources.TASK));

	  /// <summary>
	  /// Indicates that TASK_ASSIGN interactions are permitted </summary>
	  public static readonly Permissions TASK_ASSIGN = new Permissions("TASK_ASSIGN", InnerEnum.TASK_ASSIGN, "TASK_ASSIGN", 32768, java.util.EnumSet.of(Resources.PROCESS_DEFINITION, Resources.TASK));

	  /// <summary>
	  /// Indicates that MIGRATE_INSTANCE interactions are permitted </summary>
	  public static readonly Permissions MIGRATE_INSTANCE = new Permissions("MIGRATE_INSTANCE", InnerEnum.MIGRATE_INSTANCE, "MIGRATE_INSTANCE", 65536, java.util.EnumSet.of(Resources.PROCESS_DEFINITION));

	  private static readonly IList<Permissions> valueList = new List<Permissions>();

	  static Permissions()
	  {
		  valueList.Add(NONE);
		  valueList.Add(ALL);
		  valueList.Add(READ);
		  valueList.Add(UPDATE);
		  valueList.Add(CREATE);
		  valueList.Add(DELETE);
		  valueList.Add(ACCESS);
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
	  }

	  public enum InnerEnum
	  {
		  NONE,
		  ALL,
		  READ,
		  UPDATE,
		  CREATE,
		  DELETE,
		  ACCESS,
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
		  MIGRATE_INSTANCE
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  // NOTE: Please use XxxPermissions for new permissions
	  // Keep in mind to use unique permissions' ids for the same Resource
	  // TODO in case a new XxxPermissions enum is created:
	  // please adjust ResourceTypeUtil#PERMISSION_ENUMS accordingly


	  // implementation //////////////////////////

	  private string name;
	  private int id;
	  private Resource[] resourceTypes;

	  private Permissions(string name, InnerEnum innerEnum, string name, int id)
	  {
		this.name = name;
		this.id = id;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  private Permissions(string name, InnerEnum innerEnum, string name, int id, java.util.EnumSet<Resources> resourceTypes)
	  {
		this.name = name;
		this.id = id;
		this.resourceTypes = resourceTypes.toArray(new Resource[resourceTypes.size()]);

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  public override string ToString()
	  {
		return name;
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
			return resourceTypes;
		  }
	  }

	  public static Permission forName(string name)
	  {
		Permission permission = valueOf(name);
		return permission;
	  }


		public static IList<Permissions> values()
		{
			return valueList;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public static Permissions valueOf(string name)
		{
			foreach (Permissions enumInstance in Permissions.valueList)
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