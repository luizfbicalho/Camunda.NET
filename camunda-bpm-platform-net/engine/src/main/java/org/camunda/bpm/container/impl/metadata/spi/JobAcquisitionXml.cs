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
	/// <para>Java API to the JobAcquisition deployment metadata</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface JobAcquisitionXml
	{

	  /// <returns> the name of the JobExecutor. </returns>
	  string Name {get;}

	  /// <returns> the fully qualified classname of the JobExecutor to be used. </returns>
	  string JobExecutorClassName {get;}

	  /// <returns> a set of properties to configure the Job Executor. The
	  ///         properties are mapped to bean properties of the JobExecutor
	  ///         class used.
	  /// </returns>
	  /// <seealso cref= #LOCK_TIME_IN_MILLIS </seealso>
	  /// <seealso cref= #WAIT_TIME_IN_MILLIS </seealso>
	  /// <seealso cref= #MAX_JOBS_PER_ACQUISITION
	  ///  </seealso>
	  IDictionary<string, string> Properties {get;}

	}

	public static class JobAcquisitionXml_Fields
	{
	  public const string LOCK_TIME_IN_MILLIS = "lockTimeInMillis";
	  public const string WAIT_TIME_IN_MILLIS = "waitTimeInMillis";
	  public const string MAX_JOBS_PER_ACQUISITION = "maxJobsPerAcquisition";
	}

}