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
namespace org.camunda.bpm.container.impl.jboss.extension
{

	/// <summary>
	/// Constants used in the model
	/// 
	/// @author Daniel Meyer
	/// @author christian.lipphardt@camunda.com
	/// </summary>
	public interface ModelConstants
	{

	  // elements

	  // attributes
	  /// <summary>
	  /// The name of our subsystem within the model. </summary>

	}

	public static class ModelConstants_Fields
	{
	  public const string ACQUISITION_STRATEGY = "acquisition-strategy";
	  public const string DATASOURCE = "datasource";
	  public const string HISTORY_LEVEL = "history-level";
	  public const string JOB_ACQUISITION = "job-acquisition";
	  public const string JOB_ACQUISITIONS = "job-acquisitions";
	  public const string JOB_EXECUTOR = "job-executor";
	  public const string PROCESS_ENGINE = "process-engine";
	  public const string PROCESS_ENGINES = "process-engines";
	  public const string PROPERTY = "property";
	  public const string PROPERTIES = "properties";
	  public const string CONFIGURATION = "configuration";
	  public const string PLUGINS = "plugins";
	  public const string PLUGIN = "plugin";
	  public const string PLUGIN_CLASS = "class";
	  public const string DEFAULT = "default";
	  public const string NAME = "name";
	  public const string THREAD_POOL_NAME = "thread-pool-name";
	  public const string SUBSYSTEM_NAME = "camunda-bpm-platform";
	}

}