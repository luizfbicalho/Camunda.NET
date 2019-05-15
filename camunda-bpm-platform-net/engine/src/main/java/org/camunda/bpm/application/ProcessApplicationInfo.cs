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
namespace org.camunda.bpm.application
{

	/// <summary>
	/// <para>Object holding information about a deployed Process Application</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	/// <seealso cref= ProcessApplicationService#getProcessApplicationInfo(String)
	///  </seealso>
	public interface ProcessApplicationInfo
	{

	  /// <summary>
	  /// constant for the servlet context path property </summary>

	  /// <returns> the name of the process application </returns>
	  string Name {get;}

	  /// <returns> a list of <seealso cref="ProcessApplicationDeploymentInfo"/> objects that
	  ///         provide information about the deployments made by the process
	  ///         application to the process engine(s). </returns>
	  IList<ProcessApplicationDeploymentInfo> DeploymentInfo {get;}

	  /// <summary>
	  /// <para>Provides access to a list of process application-provided properties.</para>
	  /// 
	  /// <para>This class provides a set of constants for commonly-used properties</para>
	  /// </summary>
	  /// <seealso cref= ProcessApplicationInfo#PROP_SERVLET_CONTEXT_PATH </seealso>
	  IDictionary<string, string> Properties {get;}


	}

	public static class ProcessApplicationInfo_Fields
	{
	  public const string PROP_SERVLET_CONTEXT_PATH = "servletContextPath";
	}

}