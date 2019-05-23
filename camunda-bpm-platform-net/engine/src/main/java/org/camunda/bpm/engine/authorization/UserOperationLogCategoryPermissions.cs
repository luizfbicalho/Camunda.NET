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
	/// The set of built-in <seealso cref="Permission Permissions"/> for <seealso cref="Resources.OPERATION_LOG_CATEGORY User operation log based on categories"/> in Camunda BPM.
	/// 
	/// @author Tobias Metzke
	/// 
	/// </summary>
	public sealed class UserOperationLogCategoryPermissions : Permission
	{

	  /// <summary>
	  /// The none permission means 'no action', 'doing nothing'.
	  /// It does not mean that no permissions are granted. 
	  /// </summary>
	  public static readonly UserOperationLogCategoryPermissions NONE = new UserOperationLogCategoryPermissions("NONE", InnerEnum.NONE, "NONE", 0);

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
	  public static readonly UserOperationLogCategoryPermissions ALL = new UserOperationLogCategoryPermissions("ALL", InnerEnum.ALL, "ALL", int.MaxValue);

	  /// <summary>
	  /// Indicates that READ interactions on defined categories are permitted. </summary>
	  public static readonly UserOperationLogCategoryPermissions READ = new UserOperationLogCategoryPermissions("READ", InnerEnum.READ, "READ", 2);

	  /// <summary>
	  /// Indicates that DELETE interactions on defined categories are permitted. </summary>
	  public static readonly UserOperationLogCategoryPermissions DELETE = new UserOperationLogCategoryPermissions("DELETE", InnerEnum.DELETE, "DELETE", 16);

	  private static readonly IList<UserOperationLogCategoryPermissions> valueList = new List<UserOperationLogCategoryPermissions>();

	  static UserOperationLogCategoryPermissions()
	  {
		  valueList.Add(NONE);
		  valueList.Add(ALL);
		  valueList.Add(READ);
		  valueList.Add(DELETE);
	  }

	  public enum InnerEnum
	  {
		  NONE,
		  ALL,
		  READ,
		  DELETE
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private static readonly Resource[] RESOURCES = new Resource[] {Resources.OPERATION_LOG_CATEGORY};

	  private string name;
	  private int id;

	  private UserOperationLogCategoryPermissions(string name, InnerEnum innerEnum, string name, int id)
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


		public static IList<UserOperationLogCategoryPermissions> values()
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

		public static UserOperationLogCategoryPermissions valueOf(string name)
		{
			foreach (UserOperationLogCategoryPermissions enumInstance in UserOperationLogCategoryPermissions.valueList)
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