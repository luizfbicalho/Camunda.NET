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
namespace org.camunda.bpm.container.impl.spi
{
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;


	/// <summary>
	/// <para>A DeploymentOperation allows bundling multiple deployment steps into a
	/// composite operation that succeeds or fails atomically.</para>
	/// 
	/// <para>The DeploymentOperation is composed of a list of individual steps (
	/// <seealso cref="DeploymentOperationStep"/>). Each step may or may not install new
	/// services into the container. If one of the steps fails, the operation makes
	/// sure that
	/// <ul>
	///  <li>all successfully completed steps are notified by calling their
	///  <seealso cref="DeploymentOperationStep.cancelOperationStep(DeploymentOperation)"/>
	///  method.</li>
	///  <li>all services installed in the context of the operation are removed from the container.</li>
	/// </ul>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </para>
	/// </summary>
	public class DeploymentOperation
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  /// <summary>
	  /// the name of this composite operation </summary>
	  protected internal readonly string name;

	  /// <summary>
	  /// the service container </summary>
	  protected internal readonly PlatformServiceContainer serviceContainer;

	  /// <summary>
	  /// the list of steps that make up this composite operation </summary>
	  protected internal readonly IList<DeploymentOperationStep> steps;

	  /// <summary>
	  /// a list of steps that completed successfully </summary>
	  protected internal readonly IList<DeploymentOperationStep> successfulSteps = new List<DeploymentOperationStep>();

	  /// <summary>
	  /// the list of services installed by this operation. The <seealso cref="rollbackOperation()"/> must make sure
	  /// all these services are removed if the operation fails. 
	  /// </summary>
	  protected internal IList<string> installedServices = new List<string>();

	  /// <summary>
	  /// a list of attachments allows to pass state from one operation to another </summary>
	  protected internal IDictionary<string, object> attachments = new Dictionary<string, object>();

	  protected internal bool isRollbackOnFailure = true;

	  protected internal DeploymentOperationStep currentStep;

	  public DeploymentOperation(string name, PlatformServiceContainer container, IList<DeploymentOperationStep> steps)
	  {
		this.name = name;
		this.serviceContainer = container;
		this.steps = steps;
	  }

	  // getter / setters /////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <S> S getAttachment(String name)
	  public virtual S getAttachment<S>(string name)
	  {
		return (S) attachments[name];
	  }

	  public virtual void addAttachment(string name, object value)
	  {
		attachments[name] = value;
	  }

	  /// <summary>
	  /// Add a new atomic step to the composite operation.
	  /// If the operation is currently executing a step, the step is added after the current step.
	  /// </summary>
	  public virtual void addStep(DeploymentOperationStep step)
	  {
		if (currentStep != null)
		{
		  steps.Insert(steps.IndexOf(currentStep) + 1, step);
		}
		else
		{
		  steps.Add(step);
		}
	  }

	  public virtual void serviceAdded(string serviceName)
	  {
		installedServices.Add(serviceName);
	  }

	  public virtual PlatformServiceContainer ServiceContainer
	  {
		  get
		  {
			return serviceContainer;
		  }
	  }

	  // runtime aspect ///////////////////////////////////

	  public virtual void execute()
	  {

		while (steps.Count > 0)
		{
		  currentStep = steps.RemoveAt(0);

		  try
		  {
			LOG.debugPerformOperationStep(currentStep.Name);

			currentStep.performOperationStep(this);
			successfulSteps.Add(currentStep);

			LOG.debugSuccessfullyPerformedOperationStep(currentStep.Name);
		  }
		  catch (Exception e)
		  {

			if (isRollbackOnFailure)
			{

			  try
			  {
				rollbackOperation();
			  }
			  catch (Exception e2)
			  {
				LOG.exceptionWhileRollingBackOperation(e2);
			  }
			  // re-throw the original exception
			  throw LOG.exceptionWhilePerformingOperationStep(name, currentStep.Name, e);
			}

			else
			{
			  LOG.exceptionWhilePerformingOperationStep(currentStep.Name, e);
			}

		  }
		}

	  }

	  protected internal virtual void rollbackOperation()
	  {

		// first, rollback all successful steps
		foreach (DeploymentOperationStep step in successfulSteps)
		{
		  try
		  {
			step.cancelOperationStep(this);
		  }
		  catch (Exception e)
		  {
			LOG.exceptionWhileRollingBackOperation(e);
		  }
		}

		// second, remove services
		foreach (string serviceName in installedServices)
		{
		  try
		  {
			serviceContainer.stopService(serviceName);
		  }
		  catch (Exception e)
		  {
			LOG.exceptionWhileStopping("service", serviceName, e);
		  }
		}
	  }

	  public virtual IList<string> InstalledServices
	  {
		  get
		  {
			return installedServices;
		  }
	  }

	  // builder /////////////////////////////

	  public class DeploymentOperationBuilder
	  {

		protected internal PlatformServiceContainer container;
		protected internal string name;
		protected internal bool isUndeploymentOperation = false;
		protected internal IList<DeploymentOperationStep> steps = new List<DeploymentOperationStep>();
		protected internal IDictionary<string, object> initialAttachments = new Dictionary<string, object>();

		public DeploymentOperationBuilder(PlatformServiceContainer container, string name)
		{
		  this.container = container;
		  this.name = name;
		}

		public virtual DeploymentOperationBuilder addStep(DeploymentOperationStep step)
		{
		  steps.Add(step);
		  return this;
		}

		public virtual DeploymentOperationBuilder addSteps(ICollection<DeploymentOperationStep> steps)
		{
		  foreach (DeploymentOperationStep step in steps)
		  {
			addStep(step);
		  }
		  return this;
		}

		public virtual DeploymentOperationBuilder addAttachment(string name, object value)
		{
		  initialAttachments[name] = value;
		  return this;
		}

		public virtual DeploymentOperationBuilder setUndeploymentOperation()
		{
		  isUndeploymentOperation = true;
		  return this;
		}

		public virtual void execute()
		{
		  DeploymentOperation operation = new DeploymentOperation(name, container, steps);
		  operation.isRollbackOnFailure = !isUndeploymentOperation;
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  operation.attachments.putAll(initialAttachments);
		  container.executeDeploymentOperation(operation);
		}

	  }


	}

}