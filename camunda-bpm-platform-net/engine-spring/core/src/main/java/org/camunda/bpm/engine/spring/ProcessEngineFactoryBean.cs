using System;

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
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using BeansException = org.springframework.beans.BeansException;
	using DisposableBean = org.springframework.beans.factory.DisposableBean;
	using FactoryBean = org.springframework.beans.factory.FactoryBean;
	using ApplicationContext = org.springframework.context.ApplicationContext;
	using ApplicationContextAware = org.springframework.context.ApplicationContextAware;

	/// <summary>
	/// @author Dave Syer
	/// @author Christian Stettler
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class ProcessEngineFactoryBean : FactoryBean<ProcessEngine>, DisposableBean, ApplicationContextAware
	{

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal ApplicationContext applicationContext;
	  protected internal ProcessEngineImpl processEngine;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void destroy() throws Exception
	  public virtual void destroy()
	  {
		if (processEngine != null)
		{
		  processEngine.close();
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setApplicationContext(org.springframework.context.ApplicationContext applicationContext) throws org.springframework.beans.BeansException
	  public virtual ApplicationContext ApplicationContext
	  {
		  set
		  {
			this.applicationContext = value;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.ProcessEngine getObject() throws Exception
	  public virtual ProcessEngine Object
	  {
		  get
		  {
			if (processEngine == null)
			{
			  initializeExpressionManager();
			  initializeTransactionExternallyManaged();
    
			  processEngine = (ProcessEngineImpl) processEngineConfiguration.buildProcessEngine();
			}
    
			return processEngine;
		  }
	  }

	  protected internal virtual void initializeExpressionManager()
	  {
		if (processEngineConfiguration.ExpressionManager == null && applicationContext != null)
		{
		  processEngineConfiguration.ExpressionManager = new SpringExpressionManager(applicationContext, processEngineConfiguration.Beans);
		}
	  }

	  protected internal virtual void initializeTransactionExternallyManaged()
	  {
		if (processEngineConfiguration is SpringProcessEngineConfiguration)
		{ // remark: any config can be injected, so we cannot have SpringConfiguration as member
		  SpringProcessEngineConfiguration engineConfiguration = (SpringProcessEngineConfiguration) processEngineConfiguration;
		  if (engineConfiguration.TransactionManager != null)
		  {
			processEngineConfiguration.TransactionsExternallyManaged = true;
		  }
		}
	  }

	  public virtual Type<ProcessEngine> ObjectType
	  {
		  get
		  {
			return typeof(ProcessEngine);
		  }
	  }

	  public virtual bool Singleton
	  {
		  get
		  {
			return true;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			return processEngineConfiguration;
		  }
		  set
		  {
			this.processEngineConfiguration = value;
		  }
	  }


	}

}