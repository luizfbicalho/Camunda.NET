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
namespace org.camunda.bpm.engine.test.api.authorization.util
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using Assert = org.junit.Assert;
	using Description = org.junit.runner.Description;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AuthorizationTestRule : AuthorizationTestBaseRule
	{

	  protected internal AuthorizationExceptionInterceptor interceptor;
	  protected internal CommandExecutor replacedCommandExecutor;

	  protected internal AuthorizationScenarioInstance scenarioInstance;

	  public AuthorizationTestRule(ProcessEngineRule engineRule) : base(engineRule)
	  {
		this.interceptor = new AuthorizationExceptionInterceptor();
	  }

	  public virtual void start(AuthorizationScenario scenario)
	  {
		start(scenario, null, new Dictionary<string, string>());
	  }

	  public virtual void start(AuthorizationScenario scenario, string userId, IDictionary<string, string> resourceBindings)
	  {
		Assert.assertNull(interceptor.LastException);
		scenarioInstance = new AuthorizationScenarioInstance(scenario, engineRule.AuthorizationService, resourceBindings);
		enableAuthorization(userId);
		interceptor.activate();
	  }

	  /// <summary>
	  /// Assert the scenario conditions. If no exception or the expected one was thrown.
	  /// </summary>
	  /// <param name="scenario"> the scenario to assert on </param>
	  /// <returns> true if no exception was thrown, false otherwise </returns>
	  public virtual bool assertScenario(AuthorizationScenario scenario)
	  {

		interceptor.deactivate();
		disableAuthorization();
		scenarioInstance.tearDown(engineRule.AuthorizationService);
		scenarioInstance.assertAuthorizationException(interceptor.LastException);
		scenarioInstance = null;

		return scenarioSucceeded();
	  }

	  /// <summary>
	  /// No exception was expected and no was thrown
	  /// </summary>
	  public virtual bool scenarioSucceeded()
	  {
		return interceptor.LastException == null;
	  }

	  public virtual bool scenarioFailed()
	  {
		return interceptor.LastException != null;
	  }

	  protected internal virtual void starting(Description description)
	  {
		ProcessEngineConfigurationImpl engineConfiguration = (ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration;

		interceptor.reset();
		engineConfiguration.CommandInterceptorsTxRequired[0].Next = interceptor;
		interceptor.Next = engineConfiguration.CommandInterceptorsTxRequired[1];

		base.starting(description);
	  }

	  protected internal override void finished(Description description)
	  {
		base.finished(description);

		ProcessEngineConfigurationImpl engineConfiguration = (ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration;

		engineConfiguration.CommandInterceptorsTxRequired[0].Next = interceptor.Next;
		interceptor.Next = null;
	  }

	  public static ICollection<AuthorizationScenario[]> asParameters(params AuthorizationScenario[] scenarios)
	  {
		IList<AuthorizationScenario[]> scenarioList = new List<AuthorizationScenario[]>();
		foreach (AuthorizationScenario scenario in scenarios)
		{
		  scenarioList.Add(new AuthorizationScenario[]{scenario});
		}

		return scenarioList;
	  }

	  public virtual AuthorizationScenarioInstanceBuilder init(AuthorizationScenario scenario)
	  {
		AuthorizationScenarioInstanceBuilder builder = new AuthorizationScenarioInstanceBuilder();
		builder.scenario = scenario;
		builder.rule = this;
		return builder;
	  }

	  public class AuthorizationScenarioInstanceBuilder
	  {
		protected internal AuthorizationScenario scenario;
		protected internal AuthorizationTestRule rule;
		protected internal string userId;
		protected internal IDictionary<string, string> resourceBindings = new Dictionary<string, string>();

		public virtual AuthorizationScenarioInstanceBuilder withUser(string userId)
		{
		  this.userId = userId;
		  return this;
		}

		public virtual AuthorizationScenarioInstanceBuilder bindResource(string key, string value)
		{
		  resourceBindings[key] = value;
		  return this;
		}

		public virtual void start()
		{
		  rule.start(scenario, userId, resourceBindings);
		}
	  }

	}

}