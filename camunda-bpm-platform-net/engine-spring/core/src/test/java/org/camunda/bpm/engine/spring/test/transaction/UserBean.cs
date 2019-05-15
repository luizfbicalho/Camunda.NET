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
namespace org.camunda.bpm.engine.spring.test.transaction
{

	using Required = org.springframework.beans.factory.annotation.Required;
	using JdbcTemplate = org.springframework.jdbc.core.JdbcTemplate;
	using Transactional = org.springframework.transaction.annotation.Transactional;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class UserBean
	{

	  /// <summary>
	  /// injected by Spring </summary>
	  private RuntimeService runtimeService;

	  /// <summary>
	  /// injected by Spring </summary>
	  private TaskService taskService;

	  /// <summary>
	  /// injected by Spring </summary>
	  private DataSource dataSource;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transactional public void hello()
	  public virtual void hello()
	  {
		// here you can do transactional stuff in your domain model
		// and it will be combined in the same transaction as 
		// the startProcessInstanceByKey to the Activiti RuntimeService
		runtimeService.startProcessInstanceByKey("helloProcess");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transactional public void completeTask(String taskId)
	  public virtual void completeTask(string taskId)
	  {

		// First insert a record in the MY_TABLE table
		JdbcTemplate jdbcTemplate = new JdbcTemplate(dataSource);
		int nrOfRows = jdbcTemplate.update("insert into MY_TABLE values ('test');");
		if (nrOfRows != 1)
		{
		  throw new Exception("Insert into MY_TABLE failed");
		}

		taskService.complete(taskId);
	  }

	  // getters and setters //////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Required public void setRuntimeService(org.camunda.bpm.engine.RuntimeService runtimeService)
	  public virtual RuntimeService RuntimeService
	  {
		  set
		  {
			this.runtimeService = value;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Required public void setTaskService(org.camunda.bpm.engine.TaskService taskService)
	  public virtual TaskService TaskService
	  {
		  set
		  {
			this.taskService = value;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Required public void setDataSource(javax.sql.DataSource dataSource)
	  public virtual DataSource DataSource
	  {
		  set
		  {
			this.dataSource = value;
		  }
	  }

	}

}