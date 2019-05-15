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
namespace org.camunda.bpm.container.impl.spi
{

	/// <summary>
	/// The service types managed by this container.
	/// 
	/// </summary>
	public sealed class ServiceTypes : ServiceType
	{

	  public static readonly ServiceTypes BPM_PLATFORM = new ServiceTypes("BPM_PLATFORM", InnerEnum.BPM_PLATFORM, "org.camunda.bpm.platform");
	  public static readonly ServiceTypes PROCESS_ENGINE = new ServiceTypes("PROCESS_ENGINE", InnerEnum.PROCESS_ENGINE, "org.camunda.bpm.platform.process-engine");
	  public static readonly ServiceTypes JOB_EXECUTOR = new ServiceTypes("JOB_EXECUTOR", InnerEnum.JOB_EXECUTOR, "org.camunda.bpm.platform.job-executor");
	  public static readonly ServiceTypes PROCESS_APPLICATION = new ServiceTypes("PROCESS_APPLICATION", InnerEnum.PROCESS_APPLICATION, "org.camunda.bpm.platform.job-executor.process-application");

	  private static readonly IList<ServiceTypes> valueList = new List<ServiceTypes>();

	  static ServiceTypes()
	  {
		  valueList.Add(BPM_PLATFORM);
		  valueList.Add(PROCESS_ENGINE);
		  valueList.Add(JOB_EXECUTOR);
		  valueList.Add(PROCESS_APPLICATION);
	  }

	  public enum InnerEnum
	  {
		  BPM_PLATFORM,
		  PROCESS_ENGINE,
		  JOB_EXECUTOR,
		  PROCESS_APPLICATION
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  protected internal string serviceRealm;

	  private ServiceTypes(string name, InnerEnum innerEnum, string serviceRealm)
	  {
		this.serviceRealm = serviceRealm;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  public string TypeName
	  {
		  get
		  {
			return serviceRealm;
		  }
	  }


		public static IList<ServiceTypes> values()
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

		public static ServiceTypes valueOf(string name)
		{
			foreach (ServiceTypes enumInstance in ServiceTypes.valueList)
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