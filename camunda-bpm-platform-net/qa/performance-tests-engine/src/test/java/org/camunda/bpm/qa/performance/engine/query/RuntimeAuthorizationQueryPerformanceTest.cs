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
namespace org.camunda.bpm.qa.performance.engine.query
{

	using AuthorizationService = org.camunda.bpm.engine.AuthorizationService;
	using HistoryService = org.camunda.bpm.engine.HistoryService;
	using ManagementService = org.camunda.bpm.engine.ManagementService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using Query = org.camunda.bpm.engine.query.Query;
	using PerfTestRunContext = org.camunda.bpm.qa.performance.engine.framework.PerfTestRunContext;
	using PerfTestStepBehavior = org.camunda.bpm.qa.performance.engine.framework.PerfTestStepBehavior;
	using AuthorizationPerformanceTestCase = org.camunda.bpm.qa.performance.engine.junit.AuthorizationPerformanceTestCase;
	using PerfTestProcessEngine = org.camunda.bpm.qa.performance.engine.junit.PerfTestProcessEngine;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	using static org.camunda.bpm.engine.authorization.Resources;
	using static org.camunda.bpm.engine.authorization.Permissions;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") @RunWith(Parameterized.class) public class RuntimeAuthorizationQueryPerformanceTest extends org.camunda.bpm.qa.performance.engine.junit.AuthorizationPerformanceTestCase
	public class RuntimeAuthorizationQueryPerformanceTest : AuthorizationPerformanceTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public static String name;
		public static string name;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public static org.camunda.bpm.engine.query.Query query;
	  public static Query query;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(2) public static org.camunda.bpm.engine.authorization.Resource resource;
	  public static Resource resource;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(3) public static org.camunda.bpm.engine.authorization.Permission[] permissions;
	  public static Permission[] permissions;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(4) public static org.camunda.bpm.engine.impl.identity.Authentication authentication;
	  public static Authentication authentication;

	  internal static IList<object[]> queryResourcesAndPermissions;

	  internal static IList<Authentication> authentications;

	  static RuntimeAuthorizationQueryPerformanceTest()
	  {
		ProcessEngine processEngine = PerfTestProcessEngine.Instance;
		RuntimeService runtimeService = processEngine.RuntimeService;
		TaskService taskService = processEngine.TaskService;

		queryResourcesAndPermissions = Arrays.asList<object[]>(new object[]
		{
			"ProcessInstanceQuery", runtimeService.createProcessInstanceQuery(), PROCESS_INSTANCE, new Permission[] {READ}
		},
		new object[]
		{
			"VariableInstanceQuery", runtimeService.createVariableInstanceQuery(), PROCESS_INSTANCE, new Permission[] {READ}
		},
		new object[]
		{
			"TaskQuery", taskService.createTaskQuery(), TASK, new Permission[] {READ}
		});

		authentications = Arrays.asList(new AuthenticationAnonymousInnerClass(System.Linq.Enumerable.Empty<string>())
		   , new AuthenticationAnonymousInnerClass2(System.Linq.Enumerable.Empty<string>())
		   , new AuthenticationAnonymousInnerClass3(Arrays.asList("g0", "g1"))
		   , new AuthenticationAnonymousInnerClass4(Arrays.asList("g0", "g1", "g2", "g3", "g4", "g5", "g6", "g7", "g8", "g9")));

	  }

	  private class AuthenticationAnonymousInnerClass : Authentication
	  {
		  public AuthenticationAnonymousInnerClass(UnknownType emptyList) : base(null, emptyList<string>)
		  {
		  }

		  public override string ToString()
		  {
			return "without authentication";
		  }
	  }

	  private class AuthenticationAnonymousInnerClass2 : Authentication
	  {
		  public AuthenticationAnonymousInnerClass2(UnknownType emptyList) : base("test", emptyList<string>)
		  {
		  }

		  public override string ToString()
		  {
			return "with authenticated user without groups";
		  }
	  }

	  private class AuthenticationAnonymousInnerClass3 : Authentication
	  {
		  public AuthenticationAnonymousInnerClass3(UnknownType asList) : base("test", asList)
		  {
		  }

		  public override string ToString()
		  {
			return "with authenticated user and 2 groups";
		  }
	  }

	  private class AuthenticationAnonymousInnerClass4 : Authentication
	  {
		  public AuthenticationAnonymousInnerClass4(UnknownType asList) : base("test", asList)
		  {
		  }

		  public override string ToString()
		  {
			return "with authenticated user and 10 groups";
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="{0} - {4}") public static Iterable<Object[]> params()
	  public static IEnumerable<object[]> @params()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<Object[]> params = new java.util.ArrayList<Object[]>();
		List<object[]> @params = new List<object[]>();

		foreach (object[] queryResourcesAndPermission in queryResourcesAndPermissions)
		{
		  foreach (Authentication authentication in authentications)
		  {
			object[] array = new object[queryResourcesAndPermission.Length + 1];
			Array.Copy(queryResourcesAndPermission, 0, array, 0, queryResourcesAndPermission.Length);
			array[queryResourcesAndPermission.Length] = authentication;
			@params.Add(array);
		  }
		}

		return @params;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createAuthorizations()
	  public virtual void createAuthorizations()
	  {
		AuthorizationService authorizationService = engine.AuthorizationService;
		IList<Authorization> auths = authorizationService.createAuthorizationQuery().list();
		foreach (Authorization authorization in auths)
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}

		userGrant("test", resource, permissions);
		for (int i = 0; i < 5; i++)
		{
		  grouptGrant("g" + i, resource, permissions);
		}
		engine.ProcessEngineConfiguration.AuthorizationEnabled = true;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryList()
	  public virtual void queryList()
	  {
		performanceTest().step(new PerfTestStepBehaviorAnonymousInnerClass(this))
	   .run();
	  }

	  private class PerfTestStepBehaviorAnonymousInnerClass : PerfTestStepBehavior
	  {
		  private readonly RuntimeAuthorizationQueryPerformanceTest outerInstance;

		  public PerfTestStepBehaviorAnonymousInnerClass(RuntimeAuthorizationQueryPerformanceTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public void execute(PerfTestRunContext context)
		  {
			try
			{
			  outerInstance.engine.IdentityService.Authentication = authentication;
			  query.listPage(0, 15);
			}
			finally
			{
			  outerInstance.engine.IdentityService.clearAuthentication();
			}
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void queryCount()
	  public virtual void queryCount()
	  {
		performanceTest().step(new PerfTestStepBehaviorAnonymousInnerClass2(this))
	   .run();
	  }

	  private class PerfTestStepBehaviorAnonymousInnerClass2 : PerfTestStepBehavior
	  {
		  private readonly RuntimeAuthorizationQueryPerformanceTest outerInstance;

		  public PerfTestStepBehaviorAnonymousInnerClass2(RuntimeAuthorizationQueryPerformanceTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public void execute(PerfTestRunContext context)
		  {
			try
			{
			  outerInstance.engine.IdentityService.Authentication = authentication;
			  query.count();
			}
			finally
			{
			  outerInstance.engine.IdentityService.clearAuthentication();
			}
		  }
	  }

	}

}