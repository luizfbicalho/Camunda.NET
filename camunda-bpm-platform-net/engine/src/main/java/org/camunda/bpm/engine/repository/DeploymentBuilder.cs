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
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using DmnModelInstance = org.camunda.bpm.model.dmn.DmnModelInstance;

	/// <summary>
	/// <para>Builder for creating new deployments.</para>
	/// 
	/// <para>A builder instance can be obtained through <seealso cref="org.camunda.bpm.engine.RepositoryService.createDeployment()"/>.</para>
	/// 
	/// <para>Multiple resources can be added to one deployment before calling the <seealso cref="deploy()"/>
	/// operation.</para>
	/// 
	/// <para>After deploying, no more changes can be made to the returned deployment
	/// and the builder instance can be disposed.</para>
	/// 
	/// <para>In order the resources to be processed as definitions, their names must have one of allowed suffixes (file extensions in case of file reference).</para>
	/// <table>
	/// <thead>
	///   <tr><th>Resource name suffix</th><th>Will be treated as</th></tr>
	/// <thead>
	/// <tbody>
	///    <tr>
	///      <td>.bpmn20.xml, .bpmn</td><td>BPMN process definition</td>
	///    </tr>
	///    <tr>
	///      <td>.cmmn11.xml, .cmmn10.xml, .cmmn</td><td>CMMN case definition</td>
	///    </tr>
	///    <tr>
	///      <td>.dmn11.xml, .dmn</td><td>DMN decision table</td>
	///    </tr>
	/// </tbody>
	/// </table>
	/// 
	/// <para>Additionally resources with resource name suffixes .png, .jpg, .gif and .svg can be treated as diagram images. The deployment resource is considered
	/// to represent the specific model diagram by file name, e.g. bpmnDiagram1.png will be considered to be a diagram image for bpmnDiagram1.bpmn20.xml.</para>
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public interface DeploymentBuilder
	{

	  DeploymentBuilder addInputStream(string resourceName, Stream inputStream);
	  DeploymentBuilder addClasspathResource(string resource);
	  DeploymentBuilder addString(string resourceName, string text);

	  /// <summary>
	  /// Adds a BPMN model to the deployment. </summary> </param>
	  /// <param name="resourceName"> resource name. See suffix requirements for resource names: {<seealso cref= DeploymentBuilder}. </seealso>
	  /// <param name="modelInstance"> model instance
	  /// @return </param>
	  DeploymentBuilder addModelInstance(string resourceName, BpmnModelInstance modelInstance);

	  /// <summary>
	  /// Adds a DMN model to the deployment. </summary> </param>
	  /// <param name="resourceName"> resource name. See suffix requirements for resource names: {<seealso cref= DeploymentBuilder}. </seealso>
	  /// <param name="modelInstance"> model instance
	  /// @return </param>
	  DeploymentBuilder addModelInstance(string resourceName, DmnModelInstance modelInstance);

	  /// <summary>
	  /// Adds a CMMN model to the deployment. </summary> </param>
	  /// <param name="resourceName"> resource name. See suffix requirements for resource names: {<seealso cref= DeploymentBuilder}. </seealso>
	  /// <param name="modelInstance"> model instance
	  /// @return </param>
	  DeploymentBuilder addModelInstance(string resourceName, CmmnModelInstance modelInstance);

	  DeploymentBuilder addZipInputStream(ZipInputStream zipInputStream);

	  /// <summary>
	  /// All existing resources contained by the given deployment
	  /// will be added to the new deployment to re-deploy them.
	  /// </summary>
	  /// <exception cref="NotValidException"> if deployment id is null. </exception>
	  DeploymentBuilder addDeploymentResources(string deploymentId);

	  /// <summary>
	  /// A given resource specified by id and deployment id will be added
	  /// to the new deployment to re-deploy the given resource.
	  /// </summary>
	  /// <exception cref="NotValidException"> if either deployment id or resource id is null. </exception>
	  DeploymentBuilder addDeploymentResourceById(string deploymentId, string resourceId);

	  /// <summary>
	  /// All given resources specified by id and deployment id will be added
	  /// to the new deployment to re-deploy the given resource.
	  /// </summary>
	  /// <exception cref="NotValidException"> if either deployment id or the list of resource ids is null. </exception>
	  DeploymentBuilder addDeploymentResourcesById(string deploymentId, IList<string> resourceIds);

	  /// <summary>
	  /// A given resource specified by name and deployment id will be added
	  /// to the new deployment to re-deploy the given resource.
	  /// </summary>
	  /// <exception cref="NotValidException"> if either deployment id or resource name is null. </exception>
	  DeploymentBuilder addDeploymentResourceByName(string deploymentId, string resourceName);

	  /// <summary>
	  /// All given resources specified by name and deployment id will be added
	  /// to the new deployment to re-deploy the given resource.
	  /// </summary>
	  /// <exception cref="NotValidException"> if either deployment id or the list of resource names is null. </exception>
	  DeploymentBuilder addDeploymentResourcesByName(string deploymentId, IList<string> resourceNames);

	  /// <summary>
	  /// Gives the deployment the given name.
	  /// </summary>
	  /// <exception cref="NotValidException">
	  ///    if <seealso cref="nameFromDeployment(string)"/> has been called before. </exception>
	  DeploymentBuilder name(string name);

	  /// <summary>
	  /// Sets the deployment id to retrieve the deployment name from it.
	  /// </summary>
	  /// <exception cref="NotValidException">
	  ///    if <seealso cref="name(string)"/> has been called before. </exception>
	  DeploymentBuilder nameFromDeployment(string deploymentId);

	  /// <summary>
	  /// <para>If set, this deployment will be compared to any previous deployment.
	  /// This means that every (non-generated) resource will be compared with the
	  /// provided resources of this deployment. If any resource of this deployment
	  /// is different to the existing resources, <i>all</i> resources are re-deployed.
	  /// </para>
	  /// 
	  /// <para><b>Deprecated</b>: use <seealso cref="enableDuplicateFiltering(bool)"/></para>
	  /// </summary>
	  [Obsolete]
	  DeploymentBuilder enableDuplicateFiltering();

	  /// <summary>
	  /// Check the resources for duplicates in the set of previous deployments with
	  /// same deployment source. If no resources have changed in this deployment,
	  /// its contained resources are not deployed at all. For further configuration,
	  /// use the parameter <code>deployChangedOnly</code>.
	  /// </summary>
	  /// <param name="deployChangedOnly"> determines whether only those resources should be
	  /// deployed that have changed from the previous versions of the deployment.
	  /// If false, all of the resources are re-deployed if any resource differs. </param>
	  DeploymentBuilder enableDuplicateFiltering(bool deployChangedOnly);

	  /// <summary>
	  /// Sets the date on which the process definitions contained in this deployment
	  /// will be activated. This means that all process definitions will be deployed
	  /// as usual, but they will be suspended from the start until the given activation date.
	  /// </summary>
	  DeploymentBuilder activateProcessDefinitionsOn(DateTime date);

	  /// <summary>
	  /// <para>Sets the source of a deployment.</para>
	  /// 
	  /// <para>
	  /// Furthermore if duplicate check of deployment resources is enabled (by calling
	  /// <seealso cref="enableDuplicateFiltering(bool)"/>) then only previous deployments
	  /// with the same given source are considered to perform the duplicate check.
	  /// </para>
	  /// </summary>
	  DeploymentBuilder source(string source);

	  /// <summary>
	  /// <para>Deploys all provided sources to the process engine and returns the created deployment.</para>
	  /// 
	  /// 
	  /// <para> The returned <seealso cref="Deployment"/> instance has no information about the definitions, which are deployed
	  /// with that deployment. To access this information you can use the <seealso cref="deployWithResult()"/> method.
	  /// This method will return an instance of <seealso cref="DeploymentWithDefinitions"/>, which contains the information
	  /// about the successful deployed definitions.
	  /// </para>
	  /// </summary>
	  /// <exception cref="NotFoundException"> thrown
	  ///  <ul>
	  ///    <li>if the deployment specified by <seealso cref="nameFromDeployment(string)"/> does not exist or</li>
	  ///    <li>if at least one of given deployments provided by <seealso cref="addDeploymentResources(string)"/> does not exist.</li>
	  ///  </ul>
	  /// </exception>
	  /// <exception cref="NotValidException">
	  ///    if there are duplicate resource names from different deployments to re-deploy.
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///  thrown if the current user does not possess the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.CREATE"/> on <seealso cref="Resources.DEPLOYMENT"/></li>
	  ///     <li><seealso cref="Permissions.READ"/> on <seealso cref="Resources.DEPLOYMENT"/> (if resources from previous deployments are redeployed)</li>
	  ///   </ul> </exception>
	  /// <returns> the created deployment </returns>
	  Deployment deploy();

	  /// <summary>
	  /// Deploys all provided sources to the process engine and returns the created deployment with the deployed definitions.
	  /// </summary>
	  /// <exception cref="NotFoundException"> thrown
	  ///  <ul>
	  ///    <li>if the deployment specified by <seealso cref="nameFromDeployment(string)"/> does not exist or</li>
	  ///    <li>if at least one of given deployments provided by <seealso cref="addDeploymentResources(string)"/> does not exist.</li>
	  ///  </ul>
	  /// </exception>
	  /// <exception cref="NotValidException">
	  ///    if there are duplicate resource names from different deployments to re-deploy.
	  /// </exception>
	  /// <exception cref="AuthorizationException">
	  ///  thrown if the current user does not possess the following permissions:
	  ///   <ul>
	  ///     <li><seealso cref="Permissions.CREATE"/> on <seealso cref="Resources.DEPLOYMENT"/></li>
	  ///     <li><seealso cref="Permissions.READ"/> on <seealso cref="Resources.DEPLOYMENT"/> (if resources from previous deployments are redeployed)</li>
	  ///   </ul> </exception>
	  /// <returns> the created deployment, contains the deployed definitions </returns>
	  DeploymentWithDefinitions deployWithResult();

	  ///  <returns> the names of the resources which were added to this builder. </returns>
	  ICollection<string> ResourceNames {get;}

	  /// <summary>
	  /// Sets the tenant id of a deployment.
	  /// </summary>
	  DeploymentBuilder tenantId(string tenantId);

	}

}