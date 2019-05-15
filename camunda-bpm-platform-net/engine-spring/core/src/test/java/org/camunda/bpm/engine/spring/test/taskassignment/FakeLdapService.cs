﻿using System;
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
namespace org.camunda.bpm.engine.spring.test.taskassignment
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class FakeLdapService
	{

	  public virtual string findManagerForEmployee(string employee)
	  {
		// Pretty useless LDAP service ...
		return "Kermit The Frog";
	  }

	  public virtual IList<string> findAllSales()
	  {
		return Arrays.asList("kermit", "gonzo", "fozzie");
	  }

	  public virtual IList<string> findManagers(DelegateExecution execution, string emp)
	  {
		if (execution == null)
		{
		  throw new Exception("Execution parameter is null");
		}

		if (string.ReferenceEquals(emp, null) || "".Equals(emp))
		{
		  throw new Exception("emp parameter is null or empty");
		}

		return Arrays.asList("management", "directors");
	  }

	}

}