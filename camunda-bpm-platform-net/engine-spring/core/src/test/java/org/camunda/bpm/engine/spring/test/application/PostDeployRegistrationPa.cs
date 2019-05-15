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
namespace org.camunda.bpm.engine.spring.test.application
{

	using PostDeploy = org.camunda.bpm.application.PostDeploy;
	using PreUndeploy = org.camunda.bpm.application.PreUndeploy;
	using ProcessApplicationExecutionException = org.camunda.bpm.application.ProcessApplicationExecutionException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using SpringProcessApplication = org.camunda.bpm.engine.spring.application.SpringProcessApplication;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PostDeployRegistrationPa : SpringProcessApplication
	{

	  protected internal bool isPostDeployInvoked = false;
	  protected internal bool isPreUndeployInvoked = false;
	  protected internal string deploymentId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PostDeploy public void registerProcessApplication(org.camunda.bpm.engine.ProcessEngine processEngine)
	  public virtual void registerProcessApplication(ProcessEngine processEngine)
	  {

		// lookup existing deployment
		ProcessDefinition processDefinition = processEngine.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("startToEnd").latestVersion().singleResult();

		deploymentId = processDefinition.DeploymentId;

		// register with the process engine
		processEngine.ManagementService.registerProcessApplication(deploymentId, Reference);


		isPostDeployInvoked = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PreUndeploy public void unregisterProcessApplicaiton(org.camunda.bpm.engine.ProcessEngine processEngine)
	  public virtual void unregisterProcessApplicaiton(ProcessEngine processEngine)
	  {

		// unregister with the process engine
		processEngine.ManagementService.unregisterProcessApplication(deploymentId, true);

		isPreUndeployInvoked = true;

	  }


	  // customization of Process Application for unit test ////////////////////////////

	  protected internal bool isInvoked = false;

	  public override void start()
	  {
		// do not auto-deploy the process application : we want to manually deploy
		// from the test-case
	  }

	  /// <summary>
	  /// override execute to intercept calls from process engine and record that we are invoked. </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public <T> T execute(java.util.concurrent.Callable<T> callable) throws org.camunda.bpm.application.ProcessApplicationExecutionException
	  public override T execute<T>(Callable<T> callable)
	  {
		T result = base.execute(callable);
		isInvoked = true;
		return result;
	  }

	  public virtual bool Invoked
	  {
		  get
		  {
			return isInvoked;
		  }
	  }

	  public virtual bool PostDeployInvoked
	  {
		  get
		  {
			return isPostDeployInvoked;
		  }
	  }

	  public virtual bool PreUndeployInvoked
	  {
		  get
		  {
			return isPreUndeployInvoked;
		  }
	  }

	}

}