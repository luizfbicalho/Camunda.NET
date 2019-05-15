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
namespace org.camunda.bpm.engine.test.api.runtime
{
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using CachedDbEntity = org.camunda.bpm.engine.impl.db.entitymanager.cache.CachedDbEntity;
	using DbEntityState = org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityState;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Test = org.junit.Test;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class VariableInTransactionTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateAndDeleteVariableInTransaction() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCreateAndDeleteVariableInTransaction()
	  {

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));

	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly VariableInTransactionTest outerInstance;

		  public CommandAnonymousInnerClass(VariableInTransactionTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			//create a variable
			VariableInstanceEntity variable = VariableInstanceEntity.createAndInsert("aVariable", Variables.byteArrayValue(new sbyte[0]));
			string byteArrayId = variable.ByteArrayValueId;

			//delete the variable
			variable.delete();

			//check if the variable is deleted transient
			//-> no insert and delete stmt will be flushed
			DbEntityManager dbEntityManager = commandContext.DbEntityManager;
			CachedDbEntity cachedEntity = dbEntityManager.DbEntityCache.getCachedEntity(typeof(ByteArrayEntity), byteArrayId);

			DbEntityState entityState = cachedEntity.EntityState;
			assertEquals(DbEntityState.DELETED_TRANSIENT, entityState);

			return null;
		  }
	  }
	}

}