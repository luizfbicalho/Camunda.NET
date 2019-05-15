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
namespace org.camunda.bpm.application.impl.metadata.spi
{

	using ResumePreviousBy = org.camunda.bpm.engine.repository.ResumePreviousBy;

	/// <summary>
	/// <para>Java API representation of a ProcessArchive definition inside a processes.xml file</para>
	/// 
	/// @author Daniel Meyer
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public interface ProcessArchiveXml
	{

	  /// <summary>
	  /// Indicates whether the undeployment of the process archive should trigger deleting the process engine deployment.
	  /// If the process engine deployment is deleted, all running and historic process instances are removed as well. 
	  /// </summary>

	  /// <summary>
	  /// Indicates whether the classloader should be scanned for process definitions. </summary>

	  /// <summary>
	  /// Indicates whether old versions of the deployment should be resumed.
	  /// If this property is not set, the default value is used: true. 
	  /// </summary>

	  /// <summary>
	  /// Indicates which previous deployments should be resumed by this deployment.
	  /// Can be any of the options in <seealso cref="ResumePreviousBy"/>.
	  /// </summary>

	  /// <summary>
	  /// Indicates whether only changed resources should be part of the deployment.
	  /// This is independent of the setting that if no resources change, no deployment
	  /// takes place but the previous deployment is resumed.
	  /// </summary>

	  /// <summary>
	  /// <para> The resource root of the proccess archive. This property is used when scanning for process definitions
	  /// (if <seealso cref="#PROP_IS_SCAN_FOR_PROCESS_DEFINITIONS"/> is set to true).</para>
	  /// 
	  /// <para> The path is interpreted as
	  /// <ul>
	  /// 
	  ///   <li>
	  ///     <em>local to the root of the classpath.</em>
	  ///     By default or if the prefix "classpath:" is used, the path is interpreted as relative to the root
	  ///     of the classloader. Example: "path/to/my/processes" or "classpath:path/to/my/processes")
	  ///   </li>
	  /// 
	  ///   <li>
	  ///     <em>relative to the process archive's definig metafile (processes.xml).</em>
	  ///     If the prefix "pa:" is used, the path is interpreted as relative to the metafile defining the
	  ///     process archive. Consider the situation of a process application packaged as a WAR file:
	  /// 
	  ///     The deployment structure could look like this:
	  ///     <pre>
	  ///     |-- My-Application.war
	  ///         |-- WEB-INF
	  ///             |-- lib/
	  ///                 |-- Sales-Processes.jar
	  ///                     |-- META-INF/processes.xml  (1)
	  ///                     |-- opps/openOpportunity.bpmn
	  ///                     |-- leads/openLead.bpmn
	  /// 
	  ///                 |-- Invoice-Processes.jar
	  ///                     |-- META-INF/processes.xml  (2)
	  ///    </pre>
	  ///    If the process archive(s) defined in (1) uses a path prefixed with "pa:", like for instance "pa:opps/",
	  ///    only the "opps/"-folder of sales-processes.jar is scanned. More precisely, a "pa-local path", is resolved
	  ///    relative to the the parent directory of the META-INF-directory containing the defining processes.xml file.
	  ///    This implies, that using a pa-local path in (1), no processes from (2) are visible.
	  ///    <p />
	  ///   </li>
	  /// </ul>
	  /// </para>
	  /// </summary>

	  /// <summary>
	  /// A semicolon separated list of additional suffixes for resources to scan for.
	  /// </summary>

	  /// <returns> the name of the process archive. Must not be null. </returns>
	  string Name {get;}

	  /// <returns> the id of the tenant the resources of the process archive should deploy for. Can be <code>null</code>. </returns>
	  string TenantId {get;}

	  /// <returns> the name of the process engine which the deployment should be made to. If null, the "default engine" is used. </returns>
	  string ProcessEngineName {get;}

	  /// <returns> a list of process definition resource names that make up the deployment. </returns>
	  IList<string> ProcessResourceNames {get;}

	  /// <returns> a list of additional properties. See constant property names defined in this class for a list of available properties.
	  /// </returns>
	  /// <seealso cref= #PROP_IS_DELETE_UPON_UNDEPLOY </seealso>
	  /// <seealso cref= #PROP_IS_SCAN_FOR_PROCESS_DEFINITIONS </seealso>
	  /// <seealso cref= #PROP_RESOURCE_ROOT_PATH </seealso>
	  /// <seealso cref= #PROP_IS_DEPLOY_CHANGED_ONLY </seealso>
	  IDictionary<string, string> Properties {get;}

	}

	public static class ProcessArchiveXml_Fields
	{
	  public const string PROP_IS_DELETE_UPON_UNDEPLOY = "isDeleteUponUndeploy";
	  public const string PROP_IS_SCAN_FOR_PROCESS_DEFINITIONS = "isScanForProcessDefinitions";
	  public const string PROP_IS_RESUME_PREVIOUS_VERSIONS = "isResumePreviousVersions";
	  public const string PROP_RESUME_PREVIOUS_BY = "resumePreviousBy";
	  public const string PROP_IS_DEPLOY_CHANGED_ONLY = "isDeployChangedOnly";
	  public const string PROP_RESOURCE_ROOT_PATH = "resourceRootPath";
	  public const string PROP_ADDITIONAL_RESOURCE_SUFFIXES = "additionalResourceSuffixes";
	  public const string PROP_ADDITIONAL_RESOURCE_SUFFIXES_SEPARATOR = ",";
	}

}