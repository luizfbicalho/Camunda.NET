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
namespace org.camunda.bpm.container.impl.metadata
{

	using TestCase = junit.framework.TestCase;

	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using DefaultJobExecutor = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobExecutor;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class PropertyHelperTest : TestCase
	{

	  // process engine properties
	  protected internal const string JOB_EXECUTOR_DEPLOYMENT_AWARE_PROP = "jobExecutorDeploymentAware";
	  protected internal const string JOB_EXECUTOR_PREFER_TIMER_JOBS = "jobExecutorPreferTimerJobs";
	  protected internal const string JOB_EXECUTOR_ACQUIRE_BY_DUE_DATE = "jobExecutorAcquireByDueDate";
	  protected internal const string MAIL_SERVER_PORT_PROP = "mailServerPort";
	  protected internal const string JDBC_URL_PROP = "jdbcUrl";
	  protected internal const string DB_IDENTITY_USED_PROP = "dbIdentityUsed";

	  // job executor properties
	  protected internal const string MAX_JOBS_PER_ACQUISITION = "maxJobsPerAcquisition";
	  protected internal const string MAX_WAIT = "maxWait";
	  protected internal const string WAIT_INCREASE_FACTOR = "waitIncreaseFactor";
	  protected internal const string BACKOFF_TIME_IN_MILLIS = "backoffTimeInMillis";


	  /// <summary>
	  /// Assert that String, int and boolean properties can be set.
	  /// </summary>
	  public virtual void testProcessEngineConfigurationProperties()
	  {
		ProcessEngineConfiguration engineConfiguration = new StandaloneProcessEngineConfiguration();

		IDictionary<string, string> propertiesToSet = new Dictionary<string, string>();
		propertiesToSet[JOB_EXECUTOR_DEPLOYMENT_AWARE_PROP] = "true";
		propertiesToSet[JOB_EXECUTOR_PREFER_TIMER_JOBS] = "true";
		propertiesToSet[JOB_EXECUTOR_ACQUIRE_BY_DUE_DATE] = "true";
		propertiesToSet[MAIL_SERVER_PORT_PROP] = "42";
		propertiesToSet[JDBC_URL_PROP] = "someUrl";

		PropertyHelper.applyProperties(engineConfiguration, propertiesToSet);

		Assert.assertTrue(engineConfiguration.JobExecutorDeploymentAware);
		Assert.assertTrue(engineConfiguration.JobExecutorPreferTimerJobs);
		Assert.assertTrue(engineConfiguration.JobExecutorAcquireByDueDate);
		Assert.assertEquals(42, engineConfiguration.MailServerPort);
		Assert.assertEquals("someUrl", engineConfiguration.JdbcUrl);
	  }

	  public virtual void testJobExecutorConfigurationProperties()
	  {
		// given
		JobExecutor jobExecutor = new DefaultJobExecutor();

		IDictionary<string, string> propertiesToSet = new Dictionary<string, string>();
		propertiesToSet[MAX_JOBS_PER_ACQUISITION] = Convert.ToString(int.MaxValue);
		propertiesToSet[MAX_WAIT] = Convert.ToString(long.MaxValue);
		propertiesToSet[WAIT_INCREASE_FACTOR] = Convert.ToString(float.MaxValue);
		propertiesToSet[BACKOFF_TIME_IN_MILLIS] = Convert.ToString(int.MaxValue);

		// when
		PropertyHelper.applyProperties(jobExecutor, propertiesToSet);

		// then
		Assert.assertEquals(int.MaxValue, jobExecutor.MaxJobsPerAcquisition);
		Assert.assertEquals(long.MaxValue, jobExecutor.MaxWait);
		Assert.assertEquals(float.MaxValue, jobExecutor.WaitIncreaseFactor, 0.0001d);
		Assert.assertEquals(int.MaxValue, jobExecutor.BackoffTimeInMillis);
	  }

	  /// <summary>
	  /// Assures that property names are matched on the setter name according to java beans conventions
	  /// and not on the field name.
	  /// </summary>
	  public virtual void testConfigurationPropertiesWithMismatchingFieldAndSetter()
	  {
		ProcessEngineConfigurationImpl engineConfiguration = new StandaloneProcessEngineConfiguration();

		IDictionary<string, string> propertiesToSet = new Dictionary<string, string>();
		propertiesToSet[DB_IDENTITY_USED_PROP] = "false";
		PropertyHelper.applyProperties(engineConfiguration, propertiesToSet);

		Assert.assertFalse(engineConfiguration.DbIdentityUsed);

		propertiesToSet[DB_IDENTITY_USED_PROP] = "true";
		PropertyHelper.applyProperties(engineConfiguration, propertiesToSet);

		Assert.assertTrue(engineConfiguration.DbIdentityUsed);
	  }

	  public virtual void testNonExistingPropertyForProcessEngineConfiguration()
	  {
		ProcessEngineConfiguration engineConfiguration = new StandaloneProcessEngineConfiguration();
		IDictionary<string, string> propertiesToSet = new Dictionary<string, string>();
		propertiesToSet["aNonExistingProperty"] = "someValue";

		try
		{
		  PropertyHelper.applyProperties(engineConfiguration, propertiesToSet);
		  Assert.fail();
		}
		catch (Exception)
		{
		  // happy path
		}
	  }

	  public virtual void testResolvePropertyForExistingProperty()
	  {
		Properties source = new Properties();
		source.put("camunda.test.someKey", "1234");
		string result = PropertyHelper.resolveProperty(source, "${camunda.test.someKey}");
		Assert.assertEquals("1234", result);
	  }

	  public virtual void testResolvePropertyWhitespaceAndMore()
	  {
		Properties source = new Properties();
		source.put("camunda.test.someKey", "1234");
		string result = PropertyHelper.resolveProperty(source, " -${ camunda.test.someKey }- ");
		Assert.assertEquals(" -1234- ", result);
	  }

	  public virtual void testResolvePropertyForMultiplePropertes()
	  {
		Properties source = new Properties();
		source.put("camunda.test.oneKey", "1234");
		source.put("camunda.test.anotherKey", "5678");
		string result = PropertyHelper.resolveProperty(source, "-${ camunda.test.oneKey }-${ camunda.test.anotherKey}-");
		Assert.assertEquals("-1234-5678-", result);
	  }

	  public virtual void testResolvePropertyForMissingProperty()
	  {
		Properties source = new Properties();
		string result = PropertyHelper.resolveProperty(source, "${camunda.test.someKey}");
		Assert.assertEquals("", result);
	  }

	  public virtual void testResolvePropertyNoTemplate()
	  {
		Properties source = new Properties();
		source.put("camunda.test.someKey", "1234");
		string result = PropertyHelper.resolveProperty(source, "camunda.test.someKey");
		Assert.assertEquals("camunda.test.someKey", result);
	  }
	}

}