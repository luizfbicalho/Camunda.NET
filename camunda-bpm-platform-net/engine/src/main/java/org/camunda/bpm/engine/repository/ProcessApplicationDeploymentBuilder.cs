using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.repository
{

	using ProcessApplication = org.camunda.bpm.application.ProcessApplication;
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// <para>Builder for a <seealso cref="ProcessApplication"/> deployment</para>
	/// 
	/// <para>A process application deployment is different from a regular deployment.
	/// Besides deploying a set of process definitions to the database,
	/// this deployment has the additional side effect that the process application
	/// is registered for the deployment. This means that the process engine will exeute
	/// all process definitions contained in the deployment in the context of the process
	/// application (by calling the process application's
	/// </para>
	/// <seealso cref="ProcessApplicationInterface#execute(java.util.concurrent.Callable)"/> method.<para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </para>
	/// </summary>
	public interface ProcessApplicationDeploymentBuilder : DeploymentBuilder
	{

	  /// <summary>
	  /// <para>If this method is called, additional registrations will be created for
	  /// previous versions of the deployment.</para>
	  /// </summary>
	  ProcessApplicationDeploymentBuilder resumePreviousVersions();

	  /// <summary>
	  /// This method defines on what additional registrations will be based.
	  /// The value will only be recognized if <seealso cref="#resumePreviousVersions()"/> is set.
	  /// <para>
	  /// </para>
	  /// </summary>
	  /// <seealso cref= ResumePreviousBy </seealso>
	  /// <seealso cref= #resumePreviousVersions() </seealso>
	  /// <param name="resumeByProcessDefinitionKey"> one of the constants from <seealso cref="ResumePreviousBy"/> </param>
	  ProcessApplicationDeploymentBuilder resumePreviousVersionsBy(string resumePreviousVersionsBy);

	  /* {@inheritDoc} */
	  ProcessApplicationDeployment deploy();

	  // overridden methods //////////////////////////////

	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addInputStream(string resourceName, Stream inputStream);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addClasspathResource(string resource);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addString(string resourceName, string text);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addModelInstance(string resourceName, BpmnModelInstance modelInstance);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addZipInputStream(ZipInputStream zipInputStream);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder name(string name);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder nameFromDeployment(string deploymentId);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder source(string source);
	  /* {@inheritDoc} */
	  [Obsolete]
	  ProcessApplicationDeploymentBuilder enableDuplicateFiltering();
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder enableDuplicateFiltering(bool deployChangedOnly);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder activateProcessDefinitionsOn(DateTime date);

	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addDeploymentResources(string deploymentId);

	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addDeploymentResourceById(string deploymentId, string resourceId);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addDeploymentResourcesById(string deploymentId, IList<string> resourceIds);

	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addDeploymentResourceByName(string deploymentId, string resourceName);
	  /* {@inheritDoc} */
	  ProcessApplicationDeploymentBuilder addDeploymentResourcesByName(string deploymentId, IList<string> resourceNames);

	}

}