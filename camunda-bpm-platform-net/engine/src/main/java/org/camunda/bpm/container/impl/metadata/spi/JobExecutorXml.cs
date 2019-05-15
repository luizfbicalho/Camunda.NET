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
namespace org.camunda.bpm.container.impl.metadata.spi
{

	/// <summary>
	/// <para>Deployment Metadata for the JobExecutor Service.</para>
	/// 
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface JobExecutorXml
	{

	  /// <summary>
	  /// The time in milliseconds that threads over <seealso cref="#CORE_POOL_SIZE"/> will be kept alive.
	  /// </summary>
	  /// <returns> a list of configured JobAcquisitions. </returns>
	  IList<JobAcquisitionXml> JobAcquisitions {get;}

	  /// <returns> a set of properties to configure the Job Executor.
	  /// </returns>
	  /// <seealso cref= #QUEUE_SIZE </seealso>
	  /// <seealso cref= #CORE_POOL_SIZE </seealso>
	  /// <seealso cref= #MAX_POOL_SIZE </seealso>
	  /// <seealso cref= #KEEP_ALIVE_TIME
	  ///  </seealso>
	  IDictionary<string, string> Properties {get;}
	}

	public static class JobExecutorXml_Fields
	{
	  public const string QUEUE_SIZE = "queueSize";
	  public const string CORE_POOL_SIZE = "corePoolSize";
	  public const string MAX_POOL_SIZE = "maxPoolSize";
	  public const string KEEP_ALIVE_TIME = "keepAliveTime";
	}

}