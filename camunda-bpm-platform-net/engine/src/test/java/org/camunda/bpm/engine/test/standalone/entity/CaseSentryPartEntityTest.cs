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
namespace org.camunda.bpm.engine.test.standalone.entity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using CaseSentryPartEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartEntity;
	using CaseSentryPartQueryImpl = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartQueryImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// @author Kristin Polenz
	/// </summary>
	public class CaseSentryPartEntityTest : PluggableProcessEngineTestCase
	{

	  public virtual void testSentryWithTenantId()
	  {
		CaseSentryPartEntity caseSentryPartEntity = new CaseSentryPartEntity();
		caseSentryPartEntity.TenantId = "tenant1";

		insertCaseSentryPart(caseSentryPartEntity);

		caseSentryPartEntity = readCaseSentryPart();
		assertThat(caseSentryPartEntity.TenantId, @is("tenant1"));

		deleteCaseSentryPart(caseSentryPartEntity);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void insertCaseSentryPart(final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartEntity caseSentryPartEntity)
	  protected internal virtual void insertCaseSentryPart(CaseSentryPartEntity caseSentryPartEntity)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, caseSentryPartEntity));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly CaseSentryPartEntityTest outerInstance;

		  private CaseSentryPartEntity caseSentryPartEntity;

		  public CommandAnonymousInnerClass(CaseSentryPartEntityTest outerInstance, CaseSentryPartEntity caseSentryPartEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.caseSentryPartEntity = caseSentryPartEntity;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			commandContext.CaseSentryPartManager.insert(caseSentryPartEntity);
			return null;
		  }
	  }

	  protected internal virtual CaseSentryPartEntity readCaseSentryPart()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
		return (new CaseSentryPartQueryImpl(commandExecutor)).singleResult();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void deleteCaseSentryPart(final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartEntity caseSentryPartEntity)
	  protected internal virtual void deleteCaseSentryPart(CaseSentryPartEntity caseSentryPartEntity)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, caseSentryPartEntity));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly CaseSentryPartEntityTest outerInstance;

		  private CaseSentryPartEntity caseSentryPartEntity;

		  public CommandAnonymousInnerClass2(CaseSentryPartEntityTest outerInstance, CaseSentryPartEntity caseSentryPartEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.caseSentryPartEntity = caseSentryPartEntity;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			commandContext.CaseSentryPartManager.delete(caseSentryPartEntity);
			return null;
		  }
	  }

	}

}