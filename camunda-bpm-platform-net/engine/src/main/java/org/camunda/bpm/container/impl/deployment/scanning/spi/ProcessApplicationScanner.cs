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
namespace org.camunda.bpm.container.impl.deployment.scanning.spi
{

	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ProcessApplicationScanner
	{

	  /// 
	  /// <param name="classLoader">
	  ///          the classloader to scan </param>
	  /// <param name="paResourceRootPath">
	  ///          see <seealso cref="ProcessArchiveXml.PROP_RESOURCE_ROOT_PATH"/> </param>
	  /// <param name="metaFileUrl">
	  ///          the URL to the META-INF/processes.xml file </param>
	  /// <returns> a Map of process definitions </returns>
	  IDictionary<string, sbyte[]> findResources(ClassLoader classLoader, string paResourceRootPath, URL metaFileUrl);

	  /// 
	  /// <param name="classLoader">
	  ///          the classloader to scan </param>
	  /// <param name="paResourceRootPath">
	  ///          see <seealso cref="ProcessArchiveXml.PROP_RESOURCE_ROOT_PATH"/> </param>
	  /// <param name="metaFileUrl">
	  ///          the URL to the META-INF/processes.xml file </param>
	  /// <param name="additionalResourceSuffixes">
	  ///          a list of additional suffixes for resources </param>
	  /// <returns> a Map of process definitions </returns>
	  IDictionary<string, sbyte[]> findResources(ClassLoader classLoader, string paResourceRootPath, URL metaFileUrl, string[] additionalResourceSuffixes);

	}

}