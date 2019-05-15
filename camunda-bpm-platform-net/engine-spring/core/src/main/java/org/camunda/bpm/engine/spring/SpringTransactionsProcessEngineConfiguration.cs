using System;
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
namespace org.camunda.bpm.engine.spring
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using CommandContextInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandContextInterceptor;
	using CommandInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandInterceptor;
	using LogInterceptor = org.camunda.bpm.engine.impl.interceptor.LogInterceptor;
	using ProcessApplicationContextInterceptor = org.camunda.bpm.engine.impl.interceptor.ProcessApplicationContextInterceptor;
	using EntityManagerSession = org.camunda.bpm.engine.impl.variable.serializer.jpa.EntityManagerSession;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ByteArrayResource = org.springframework.core.io.ByteArrayResource;
	using ContextResource = org.springframework.core.io.ContextResource;
	using Resource = org.springframework.core.io.Resource;
	using TransactionAwareDataSourceProxy = org.springframework.jdbc.datasource.TransactionAwareDataSourceProxy;
	using PlatformTransactionManager = org.springframework.transaction.PlatformTransactionManager;
	using TransactionTemplate = org.springframework.transaction.support.TransactionTemplate;

	/// <summary>
	/// @author Tom Baeyens
	/// @author David Syer
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// </summary>
	public class SpringTransactionsProcessEngineConfiguration : ProcessEngineConfigurationImpl
	{

	  protected internal PlatformTransactionManager transactionManager;
	  protected internal string deploymentName = "SpringAutoDeployment";
	  protected internal Resource[] deploymentResources = new Resource[0];
	  protected internal string deploymentTenantId;
	  protected internal bool deployChangedOnly;

	  public SpringTransactionsProcessEngineConfiguration()
	  {
		transactionsExternallyManaged = true;
	  }

	  public override ProcessEngine buildProcessEngine()
	  {
		ProcessEngine processEngine = base.buildProcessEngine();
		autoDeployResources(processEngine);
		return processEngine;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Collection< ? extends org.camunda.bpm.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptorsTxRequired()
	  protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequired
	  {
		  get
		  {
			if (transactionManager == null)
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new ProcessEngineException("transactionManager is required property for SpringProcessEngineConfiguration, use " + typeof(StandaloneProcessEngineConfiguration).FullName + " otherwise");
			}
    
			IList<CommandInterceptor> defaultCommandInterceptorsTxRequired = new List<CommandInterceptor>();
			defaultCommandInterceptorsTxRequired.Add(new LogInterceptor());
			defaultCommandInterceptorsTxRequired.Add(new ProcessApplicationContextInterceptor(this));
			defaultCommandInterceptorsTxRequired.Add(new SpringTransactionInterceptor(transactionManager, TransactionTemplate.PROPAGATION_REQUIRED));
			CommandContextInterceptor commandContextInterceptor = new CommandContextInterceptor(commandContextFactory, this);
			defaultCommandInterceptorsTxRequired.Add(commandContextInterceptor);
			return defaultCommandInterceptorsTxRequired;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Collection< ? extends org.camunda.bpm.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptorsTxRequiresNew()
	  protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequiresNew
	  {
		  get
		  {
			IList<CommandInterceptor> defaultCommandInterceptorsTxRequiresNew = new List<CommandInterceptor>();
			defaultCommandInterceptorsTxRequiresNew.Add(new LogInterceptor());
			defaultCommandInterceptorsTxRequiresNew.Add(new ProcessApplicationContextInterceptor(this));
			defaultCommandInterceptorsTxRequiresNew.Add(new SpringTransactionInterceptor(transactionManager, TransactionTemplate.PROPAGATION_REQUIRES_NEW));
			CommandContextInterceptor commandContextInterceptor = new CommandContextInterceptor(commandContextFactory, this, true);
			defaultCommandInterceptorsTxRequiresNew.Add(commandContextInterceptor);
			return defaultCommandInterceptorsTxRequiresNew;
		  }
	  }

	  protected internal override void initTransactionContextFactory()
	  {
		if (transactionContextFactory == null && transactionManager != null)
		{
		  transactionContextFactory = new SpringTransactionContextFactory(transactionManager);
		}
	  }

	  protected internal override void initJpa()
	  {
		base.initJpa();
		if (jpaEntityManagerFactory != null)
		{
		  sessionFactories[typeof(EntityManagerSession)] = new SpringEntityManagerSessionFactory(jpaEntityManagerFactory, jpaHandleTransaction, jpaCloseEntityManager);
		}
	  }

	  protected internal virtual void autoDeployResources(ProcessEngine processEngine)
	  {
		if (deploymentResources != null && deploymentResources.Length > 0)
		{
		  RepositoryService repositoryService = processEngine.RepositoryService;

		  DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().enableDuplicateFiltering(deployChangedOnly).name(deploymentName).tenantId(deploymentTenantId);

		  foreach (Resource resource in deploymentResources)
		  {
			string resourceName = null;

			if (resource is ContextResource)
			{
			  resourceName = ((ContextResource) resource).PathWithinContext;

			}
			else if (resource is ByteArrayResource)
			{
			  resourceName = resource.Description;

			}
			else
			{
			  resourceName = getFileResourceName(resource);
			}

			try
			{
			  if (resourceName.EndsWith(".bar", StringComparison.Ordinal) || resourceName.EndsWith(".zip", StringComparison.Ordinal) || resourceName.EndsWith(".jar", StringComparison.Ordinal))
			  {
				deploymentBuilder.addZipInputStream(new ZipInputStream(resource.InputStream));
			  }
			  else
			  {
				deploymentBuilder.addInputStream(resourceName, resource.InputStream);
			  }
			}
			catch (IOException e)
			{
			  throw new ProcessEngineException("couldn't auto deploy resource '" + resource + "': " + e.Message, e);
			}
		  }

		  deploymentBuilder.deploy();
		}
	  }

	  protected internal virtual string getFileResourceName(Resource resource)
	  {
		try
		{
		  return resource.File.AbsolutePath;
		}
		catch (IOException)
		{
		  return resource.Filename;
		}
	  }

	  public override ProcessEngineConfigurationImpl setDataSource(DataSource dataSource)
	  {
		if (dataSource is TransactionAwareDataSourceProxy)
		{
		  return base.setDataSource(dataSource);
		}
		else
		{
		  // Wrap datasource in Transaction-aware proxy
		  DataSource proxiedDataSource = new TransactionAwareDataSourceProxy(dataSource);
		  return base.setDataSource(proxiedDataSource);
		}
	  }

	  public virtual PlatformTransactionManager TransactionManager
	  {
		  get
		  {
			return transactionManager;
		  }
		  set
		  {
			this.transactionManager = value;
		  }
	  }


	  public virtual string DeploymentName
	  {
		  get
		  {
			return deploymentName;
		  }
		  set
		  {
			this.deploymentName = value;
		  }
	  }


	  public virtual Resource[] DeploymentResources
	  {
		  get
		  {
			return deploymentResources;
		  }
		  set
		  {
			this.deploymentResources = value;
		  }
	  }


	  public virtual string DeploymentTenantId
	  {
		  get
		  {
			return deploymentTenantId;
		  }
		  set
		  {
			this.deploymentTenantId = value;
		  }
	  }


	  public virtual bool DeployChangedOnly
	  {
		  get
		  {
			return deployChangedOnly;
		  }
		  set
		  {
			this.deployChangedOnly = value;
		  }
	  }


	}

}