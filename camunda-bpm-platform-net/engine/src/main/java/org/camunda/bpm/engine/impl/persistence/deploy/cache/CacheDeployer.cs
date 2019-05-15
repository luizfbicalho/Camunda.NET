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
namespace org.camunda.bpm.engine.impl.persistence.deploy.cache
{
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;


	/// <summary>
	/// @author: Johannes Heinemann
	/// </summary>
	public class CacheDeployer
	{

	  private static readonly CommandLogger LOG = ProcessEngineLogger.CMD_LOGGER;

	  protected internal IList<Deployer> deployers;

	  public CacheDeployer()
	  {
		this.deployers = Collections.emptyList();
	  }

	  public virtual IList<Deployer> Deployers
	  {
		  set
		  {
			this.deployers = value;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void deploy(final org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity deployment)
	  public virtual void deploy(DeploymentEntity deployment)
	  {
		Context.CommandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, deployment));
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly CacheDeployer outerInstance;

		  private DeploymentEntity deployment;

		  public CallableAnonymousInnerClass(CacheDeployer outerInstance, DeploymentEntity deployment)
		  {
			  this.outerInstance = outerInstance;
			  this.deployment = deployment;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			foreach (Deployer deployer in outerInstance.deployers)
			{
			  deployer.deploy(deployment);
			}
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void deployOnlyGivenResourcesOfDeployment(final org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity deployment, String... resourceNames)
	  public virtual void deployOnlyGivenResourcesOfDeployment(DeploymentEntity deployment, params string[] resourceNames)
	  {
		initDeployment(deployment, resourceNames);
		Context.CommandContext.runWithoutAuthorization(new CallableAnonymousInnerClass2(this, deployment));
		deployment.Resources = null;
	  }

	  private class CallableAnonymousInnerClass2 : Callable<Void>
	  {
		  private readonly CacheDeployer outerInstance;

		  private DeploymentEntity deployment;

		  public CallableAnonymousInnerClass2(CacheDeployer outerInstance, DeploymentEntity deployment)
		  {
			  this.outerInstance = outerInstance;
			  this.deployment = deployment;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			foreach (Deployer deployer in outerInstance.deployers)
			{
			  deployer.deploy(deployment);
			}
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void initDeployment(final org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity deployment, String... resourceNames)
	  protected internal virtual void initDeployment(DeploymentEntity deployment, params string[] resourceNames)
	  {
		deployment.clearResources();
		foreach (string resourceName in resourceNames)
		{
		  if (!string.ReferenceEquals(resourceName, null))
		  {
			// with the given resource we prevent the deployment of querying
			// the database which means using all resources that were utilized during the deployment
			ResourceEntity resource = Context.CommandContext.ResourceManager.findResourceByDeploymentIdAndResourceName(deployment.Id, resourceName);

			deployment.addResource(resource);
		  }
		}
	  }
	}

}