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
	/// <para>The set of built-in <seealso cref="Resource"/> names.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public sealed class Resources : Resource
	{

	  public static readonly Resources APPLICATION = new Resources("APPLICATION", InnerEnum.APPLICATION, org.camunda.bpm.engine.EntityTypes.APPLICATION, 0);
	  public static readonly Resources USER = new Resources("USER", InnerEnum.USER, org.camunda.bpm.engine.EntityTypes.USER, 1);
	  public static readonly Resources GROUP = new Resources("GROUP", InnerEnum.GROUP, org.camunda.bpm.engine.EntityTypes.GROUP, 2);
	  public static readonly Resources GROUP_MEMBERSHIP = new Resources("GROUP_MEMBERSHIP", InnerEnum.GROUP_MEMBERSHIP, org.camunda.bpm.engine.EntityTypes.GROUP_MEMBERSHIP, 3);
	  public static readonly Resources AUTHORIZATION = new Resources("AUTHORIZATION", InnerEnum.AUTHORIZATION, org.camunda.bpm.engine.EntityTypes.AUTHORIZATION, 4);
	  public static readonly Resources FILTER = new Resources("FILTER", InnerEnum.FILTER, org.camunda.bpm.engine.EntityTypes.FILTER, 5);
	  public static readonly Resources PROCESS_DEFINITION = new Resources("PROCESS_DEFINITION", InnerEnum.PROCESS_DEFINITION, org.camunda.bpm.engine.EntityTypes.PROCESS_DEFINITION, 6);
	  public static readonly Resources TASK = new Resources("TASK", InnerEnum.TASK, org.camunda.bpm.engine.EntityTypes.TASK, 7);
	  public static readonly Resources PROCESS_INSTANCE = new Resources("PROCESS_INSTANCE", InnerEnum.PROCESS_INSTANCE, org.camunda.bpm.engine.EntityTypes.PROCESS_INSTANCE, 8);
	  public static readonly Resources DEPLOYMENT = new Resources("DEPLOYMENT", InnerEnum.DEPLOYMENT, org.camunda.bpm.engine.EntityTypes.DEPLOYMENT, 9);
	  public static readonly Resources DECISION_DEFINITION = new Resources("DECISION_DEFINITION", InnerEnum.DECISION_DEFINITION, org.camunda.bpm.engine.EntityTypes.DECISION_DEFINITION, 10);
	  public static readonly Resources TENANT = new Resources("TENANT", InnerEnum.TENANT, org.camunda.bpm.engine.EntityTypes.TENANT, 11);
	  public static readonly Resources TENANT_MEMBERSHIP = new Resources("TENANT_MEMBERSHIP", InnerEnum.TENANT_MEMBERSHIP, org.camunda.bpm.engine.EntityTypes.TENANT_MEMBERSHIP, 12);
	  public static readonly Resources BATCH = new Resources("BATCH", InnerEnum.BATCH, org.camunda.bpm.engine.EntityTypes.BATCH, 13);
	  public static readonly Resources DECISION_REQUIREMENTS_DEFINITION = new Resources("DECISION_REQUIREMENTS_DEFINITION", InnerEnum.DECISION_REQUIREMENTS_DEFINITION, org.camunda.bpm.engine.EntityTypes.DECISION_REQUIREMENTS_DEFINITION, 14);
	  public static readonly Resources REPORT = new Resources("REPORT", InnerEnum.REPORT, org.camunda.bpm.engine.EntityTypes.REPORT, 15);
	  public static readonly Resources DASHBOARD = new Resources("DASHBOARD", InnerEnum.DASHBOARD, org.camunda.bpm.engine.EntityTypes.DASHBOARD, 16);
	  public static readonly Resources OPERATION_LOG_CATEGORY = new Resources("OPERATION_LOG_CATEGORY", InnerEnum.OPERATION_LOG_CATEGORY, org.camunda.bpm.engine.EntityTypes.OPERATION_LOG_CATEGORY, 17);

	  private static readonly IList<Resources> valueList = new List<Resources>();

	  static Resources()
	  {
		  valueList.Add(APPLICATION);
		  valueList.Add(USER);
		  valueList.Add(GROUP);
		  valueList.Add(GROUP_MEMBERSHIP);
		  valueList.Add(AUTHORIZATION);
		  valueList.Add(FILTER);
		  valueList.Add(PROCESS_DEFINITION);
		  valueList.Add(TASK);
		  valueList.Add(PROCESS_INSTANCE);
		  valueList.Add(DEPLOYMENT);
		  valueList.Add(DECISION_DEFINITION);
		  valueList.Add(TENANT);
		  valueList.Add(TENANT_MEMBERSHIP);
		  valueList.Add(BATCH);
		  valueList.Add(DECISION_REQUIREMENTS_DEFINITION);
		  valueList.Add(REPORT);
		  valueList.Add(DASHBOARD);
		  valueList.Add(OPERATION_LOG_CATEGORY);
	  }

	  public enum InnerEnum
	  {
		  APPLICATION,
		  USER,
		  GROUP,
		  GROUP_MEMBERSHIP,
		  AUTHORIZATION,
		  FILTER,
		  PROCESS_DEFINITION,
		  TASK,
		  PROCESS_INSTANCE,
		  DEPLOYMENT,
		  DECISION_DEFINITION,
		  TENANT,
		  TENANT_MEMBERSHIP,
		  BATCH,
		  DECISION_REQUIREMENTS_DEFINITION,
		  REPORT,
		  DASHBOARD,
		  OPERATION_LOG_CATEGORY
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  internal string name;
	  internal int id;

	  internal Resources(string name, InnerEnum innerEnum, string name, int id)
	  {
		this.name = name;
		this.id = id;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  public string resourceName()
	  {
		return name;
	  }

	  public int resourceType()
	  {
		return id;
	  }


		public static IList<Resources> values()
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

		public static Resources valueOf(string name)
		{
			foreach (Resources enumInstance in Resources.valueList)
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