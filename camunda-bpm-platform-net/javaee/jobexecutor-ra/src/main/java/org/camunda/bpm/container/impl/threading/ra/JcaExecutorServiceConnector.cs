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
namespace org.camunda.bpm.container.impl.threading.ra
{


	using CommonJWorkManagerExecutorService = org.camunda.bpm.container.impl.threading.ra.commonj.CommonJWorkManagerExecutorService;
	using JobExecutionHandler = org.camunda.bpm.container.impl.threading.ra.inflow.JobExecutionHandler;
	using JobExecutionHandlerActivation = org.camunda.bpm.container.impl.threading.ra.inflow.JobExecutionHandlerActivation;
	using JobExecutionHandlerActivationSpec = org.camunda.bpm.container.impl.threading.ra.inflow.JobExecutionHandlerActivationSpec;


	/// <summary>
	/// <para>The <seealso cref="ResourceAdapter"/> responsible for bootstrapping the JcaExecutorService</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Connector(reauthenticationSupport = false, transactionSupport = TransactionSupport.TransactionSupportLevel.NoTransaction) public class JcaExecutorServiceConnector implements javax.resource.spi.ResourceAdapter, java.io.Serializable
	[Serializable]
	public class JcaExecutorServiceConnector : ResourceAdapter
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			executorServiceWrapper = new ExecutorServiceWrapper(this);
		}


	  public const string ORG_CAMUNDA_BPM_ENGINE_PROCESS_ENGINE = "org.camunda.bpm.engine.ProcessEngine";

	  /// <summary>
	  /// This class must be free of engine classes to make it possible to install
	  /// the resource adapter without shared libraries. Some deployments scenarios might
	  /// require that.
	  /// 
	  /// The wrapper class was introduced to provide more meaning to a otherwise
	  /// unspecified property.
	  /// </summary>
	  public class ExecutorServiceWrapper
	  {
		  private readonly JcaExecutorServiceConnector outerInstance;

		  public ExecutorServiceWrapper(JcaExecutorServiceConnector outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <summary>
		/// will hold a org.camunda.bpm.container.ExecutorService reference
		/// </summary>
		protected internal object executorService;

		public virtual object ExecutorService
		{
			get
			{
			  return executorService;
			}
			set
			{
			  this.executorService = value;
			}
		}


	  }

	  protected internal ExecutorServiceWrapper executorServiceWrapper;

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static Logger log = Logger.getLogger(typeof(JcaExecutorServiceConnector).FullName);

	  protected internal JobExecutionHandlerActivation jobHandlerActivation;

	  // no arg-constructor
	  public JcaExecutorServiceConnector()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  // Configuration Properties //////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConfigProperty(type = Boolean.class, defaultValue = "false", description = "If set to 'true', the CommonJ WorkManager is used instead of the Jca Work Manager." + "Can only be used on platforms where a CommonJ Workmanager is available (such as IBM & Oracle)") protected System.Nullable<bool> isUseCommonJWorkManager = false;
	  protected internal bool? isUseCommonJWorkManager = false;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConfigProperty(type=String.class, defaultValue = "wm/camunda-bpm-workmanager", description="Allows specifying the name of a CommonJ Workmanager.") protected String commonJWorkManagerName = "wm/camunda-bpm-workmanager";
	  protected internal string commonJWorkManagerName = "wm/camunda-bpm-workmanager";


	  // RA-Lifecycle ///////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start(javax.resource.spi.BootstrapContext ctx) throws javax.resource.spi.ResourceAdapterInternalException
	  public virtual void start(BootstrapContext ctx)
	  {

		try
		{
		  Type.GetType(ORG_CAMUNDA_BPM_ENGINE_PROCESS_ENGINE);
		}
		catch (Exception)
		{
		  log.info("ProcessEngine classes not found in shared libraries. Not initializing camunda Platform JobExecutor Resource Adapter.");
		  return;
		}

		// initialize the ExecutorService (CommonJ or JCA, depending on configuration)
		if (isUseCommonJWorkManager.Value)
		{
		  if (!string.ReferenceEquals(commonJWorkManagerName, null) & commonJWorkManagerName.Length > 0)
		  {
			executorServiceWrapper.ExecutorService = new CommonJWorkManagerExecutorService(this, commonJWorkManagerName);
		  }
		  else
		  {
			throw new Exception("Resource Adapter configuration property 'isUseCommonJWorkManager' is set to true but 'commonJWorkManagerName' is not provided.");
		  }

		}
		else
		{
		  executorServiceWrapper.ExecutorService = new JcaWorkManagerExecutorService(this, ctx.WorkManager);
		}

		log.log(Level.INFO, "camunda BPM executor service started.");
	  }

	  public virtual void stop()
	  {
		try
		{
		  Type.GetType(ORG_CAMUNDA_BPM_ENGINE_PROCESS_ENGINE);
		}
		catch (Exception)
		{
		  return;
		}

		log.log(Level.INFO, "camunda BPM executor service stopped.");

	  }

	  // JobHandler activation / deactivation ///////////////////////////

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endpointActivation(javax.resource.spi.endpoint.MessageEndpointFactory endpointFactory, javax.resource.spi.ActivationSpec spec) throws javax.resource.ResourceException
	  public virtual void endpointActivation(MessageEndpointFactory endpointFactory, ActivationSpec spec)
	  {
		if (jobHandlerActivation != null)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ResourceException("The camunda BPM job executor can only service a single MessageEndpoint for job execution. " + "Make sure not to deploy more than one MDB implementing the '" + typeof(JobExecutionHandler).FullName + "' interface.");
		}
		JobExecutionHandlerActivation activation = new JobExecutionHandlerActivation(this, endpointFactory, (JobExecutionHandlerActivationSpec) spec);
		activation.start();
		jobHandlerActivation = activation;
	  }

	  public virtual void endpointDeactivation(MessageEndpointFactory endpointFactory, ActivationSpec spec)
	  {
		try
		{
		  if (jobHandlerActivation != null)
		  {
			jobHandlerActivation.stop();
		  }
		}
		finally
		{
		  jobHandlerActivation = null;
		}
	  }

	  // unsupported (No TX Support) ////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.transaction.xa.XAResource[] getXAResources(javax.resource.spi.ActivationSpec[] specs) throws javax.resource.ResourceException
	  public virtual XAResource[] getXAResources(ActivationSpec[] specs)
	  {
		log.finest("getXAResources()");
		return null;
	  }

	  // getters ///////////////////////////////////////////////////////////////

	  public virtual ExecutorServiceWrapper getExecutorServiceWrapper()
	  {
		return executorServiceWrapper;
	  }

	  public virtual JobExecutionHandlerActivation JobHandlerActivation
	  {
		  get
		  {
			return jobHandlerActivation;
		  }
	  }

	  public virtual bool? IsUseCommonJWorkManager
	  {
		  get
		  {
			return isUseCommonJWorkManager;
		  }
		  set
		  {
			this.isUseCommonJWorkManager = value;
		  }
	  }


	  public virtual string CommonJWorkManagerName
	  {
		  get
		  {
			return commonJWorkManagerName;
		  }
		  set
		  {
			this.commonJWorkManagerName = value;
		  }
	  }



	  // misc //////////////////////////////////////////////////////////////////


	  public override int GetHashCode()
	  {
		return 17;
	  }

	  public override bool Equals(object other)
	  {
		if (other == null)
		{
		  return false;
		}
		if (other == this)
		{
		  return true;
		}
		if (!(other is JcaExecutorServiceConnector))
		{
		  return false;
		}
		return true;
	  }

	}

}