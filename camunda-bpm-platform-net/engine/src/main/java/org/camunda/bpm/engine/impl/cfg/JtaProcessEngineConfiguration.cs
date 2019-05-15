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
namespace org.camunda.bpm.engine.impl.cfg
{


	using JtaTransactionContextFactory = org.camunda.bpm.engine.impl.cfg.jta.JtaTransactionContextFactory;
	using StandaloneTransactionContextFactory = org.camunda.bpm.engine.impl.cfg.standalone.StandaloneTransactionContextFactory;
	using CommandContextFactory = org.camunda.bpm.engine.impl.interceptor.CommandContextFactory;
	using CommandContextInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandContextInterceptor;
	using CommandInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandInterceptor;
	using JtaTransactionInterceptor = org.camunda.bpm.engine.impl.interceptor.JtaTransactionInterceptor;
	using LogInterceptor = org.camunda.bpm.engine.impl.interceptor.LogInterceptor;
	using ProcessApplicationContextInterceptor = org.camunda.bpm.engine.impl.interceptor.ProcessApplicationContextInterceptor;
	using TxContextCommandContextFactory = org.camunda.bpm.engine.impl.interceptor.TxContextCommandContextFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JtaProcessEngineConfiguration : ProcessEngineConfigurationImpl
	{

	  private new static readonly ConfigurationLogger LOG = ProcessEngineLogger.CONFIG_LOGGER;

	  protected internal TransactionManager transactionManager;

	  protected internal string transactionManagerJndiName;

	  /// <summary>
	  /// <seealso cref="CommandContextFactory"/> to be used for DbSchemaOperations </summary>
	  protected internal CommandContextFactory dbSchemaOperationsCommandContextFactory;

	  public JtaProcessEngineConfiguration()
	  {
		transactionsExternallyManaged = true;
	  }

	  protected internal override void init()
	  {
		initTransactionManager();
		initDbSchemaOperationsCommandContextFactory();
		base.init();
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override protected java.util.Collection< ? extends org.camunda.bpm.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptorsTxRequired()
	  protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequired
	  {
		  get
		  {
			IList<CommandInterceptor> defaultCommandInterceptorsTxRequired = new List<CommandInterceptor>();
			defaultCommandInterceptorsTxRequired.Add(new LogInterceptor());
			defaultCommandInterceptorsTxRequired.Add(new ProcessApplicationContextInterceptor(this));
			defaultCommandInterceptorsTxRequired.Add(new JtaTransactionInterceptor(transactionManager, false));
			defaultCommandInterceptorsTxRequired.Add(new CommandContextInterceptor(commandContextFactory, this));
			return defaultCommandInterceptorsTxRequired;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override protected java.util.Collection< ? extends org.camunda.bpm.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptorsTxRequiresNew()
	  protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequiresNew
	  {
		  get
		  {
			IList<CommandInterceptor> defaultCommandInterceptorsTxRequiresNew = new List<CommandInterceptor>();
			defaultCommandInterceptorsTxRequiresNew.Add(new LogInterceptor());
			defaultCommandInterceptorsTxRequiresNew.Add(new ProcessApplicationContextInterceptor(this));
			defaultCommandInterceptorsTxRequiresNew.Add(new JtaTransactionInterceptor(transactionManager, true));
			defaultCommandInterceptorsTxRequiresNew.Add(new CommandContextInterceptor(commandContextFactory, this, true));
			return defaultCommandInterceptorsTxRequiresNew;
		  }
	  }

	  /// <summary>
	  /// provide custom command executor that uses NON-JTA transactions
	  /// </summary>
	  protected internal override void initCommandExecutorDbSchemaOperations()
	  {
		if (commandExecutorSchemaOperations == null)
		{
		  IList<CommandInterceptor> commandInterceptorsDbSchemaOperations = new List<CommandInterceptor>();
		  commandInterceptorsDbSchemaOperations.Add(new LogInterceptor());
		  commandInterceptorsDbSchemaOperations.Add(new CommandContextInterceptor(dbSchemaOperationsCommandContextFactory, this));
		  commandInterceptorsDbSchemaOperations.Add(actualCommandExecutor);
		  commandExecutorSchemaOperations = initInterceptorChain(commandInterceptorsDbSchemaOperations);
		}
	  }

	  protected internal virtual void initDbSchemaOperationsCommandContextFactory()
	  {
		if (dbSchemaOperationsCommandContextFactory == null)
		{
		  TxContextCommandContextFactory cmdContextFactory = new TxContextCommandContextFactory();
		  cmdContextFactory.ProcessEngineConfiguration = this;
		  cmdContextFactory.TransactionContextFactory = new StandaloneTransactionContextFactory();
		  dbSchemaOperationsCommandContextFactory = cmdContextFactory;
		}
	  }

	  protected internal virtual void initTransactionManager()
	  {
		if (transactionManager == null)
		{

		  if (string.ReferenceEquals(transactionManagerJndiName, null) || transactionManagerJndiName.Length == 0)
		  {
			throw LOG.invalidConfigTransactionManagerIsNull();
		  }

		  try
		  {
			transactionManager = (TransactionManager) (new InitialContext()).lookup(transactionManagerJndiName);

		  }
		  catch (NamingException e)
		  {
			throw LOG.invalidConfigCannotFindTransactionManger(transactionManagerJndiName + "'.", e);
		  }
		}
	  }

	  protected internal override void initTransactionContextFactory()
	  {
		if (transactionContextFactory == null)
		{
		  transactionContextFactory = new JtaTransactionContextFactory(transactionManager);
		}
	  }

	  public virtual TransactionManager TransactionManager
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


	  public virtual string TransactionManagerJndiName
	  {
		  get
		  {
			return transactionManagerJndiName;
		  }
		  set
		  {
			this.transactionManagerJndiName = value;
		  }
	  }


	  public virtual CommandContextFactory DbSchemaOperationsCommandContextFactory
	  {
		  get
		  {
			return dbSchemaOperationsCommandContextFactory;
		  }
		  set
		  {
			this.dbSchemaOperationsCommandContextFactory = value;
		  }
	  }

	}

}