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
namespace org.camunda.bpm.engine.test.standalone.db.entitymanager
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using IdGenerator = org.camunda.bpm.engine.impl.cfg.IdGenerator;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using PersistenceSession = org.camunda.bpm.engine.impl.db.PersistenceSession;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using DbEntityOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbEntityOperation;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbOperationsOrderingTest
	{

	  protected internal ExposingDbEntityManager entityManager;

	  // setup some entities
	  internal ExecutionEntity execution1 = null;
	  internal ExecutionEntity execution2 = null;
	  internal ExecutionEntity execution3 = null;
	  internal ExecutionEntity execution4 = null;
	  internal ExecutionEntity execution5 = null;
	  internal ExecutionEntity execution6 = null;
	  internal ExecutionEntity execution7 = null;
	  internal ExecutionEntity execution8 = null;

	  internal TaskEntity task1 = null;
	  internal TaskEntity task2 = null;
	  internal TaskEntity task3 = null;
	  internal TaskEntity task4 = null;

	  internal VariableInstanceEntity variable1 = null;
	  internal VariableInstanceEntity variable2 = null;
	  internal VariableInstanceEntity variable3 = null;
	  internal VariableInstanceEntity variable4 = null;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		TestIdGenerator idGenerator = new TestIdGenerator();
		entityManager = new ExposingDbEntityManager(idGenerator, null);

		execution1 = new ExecutionEntity();
		execution1.Id = "101";
		execution2 = new ExecutionEntity();
		execution2.Id = "102";
		execution3 = new ExecutionEntity();
		execution3.Id = "103";
		execution4 = new ExecutionEntity();
		execution4.Id = "104";
		execution5 = new ExecutionEntity();
		execution5.Id = "105";
		execution6 = new ExecutionEntity();
		execution6.Id = "106";
		execution7 = new ExecutionEntity();
		execution7.Id = "107";
		execution8 = new ExecutionEntity();
		execution8.Id = "108";

		task1 = new TaskEntity();
		task1.Id = "104";
		task2 = new TaskEntity();
		task2.Id = "105";
		task3 = new TaskEntity();
		task3.Id = "106";
		task4 = new TaskEntity();
		task4.Id = "107";

		variable1 = new VariableInstanceEntity();
		variable1.Id = "108";
		variable2 = new VariableInstanceEntity();
		variable2.Id = "109";
		variable3 = new VariableInstanceEntity();
		variable3.Id = "110";
		variable4 = new VariableInstanceEntity();
		variable4.Id = "111";
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInsertSingleEntity()
	  public virtual void testInsertSingleEntity()
	  {

		entityManager.insert(execution1);
		entityManager.flushEntityCache();

		IList<DbOperation> flush = entityManager.DbOperationManager.calculateFlush();
		assertEquals(1, flush.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInsertReferenceOrdering()
	  public virtual void testInsertReferenceOrdering()
	  {

		execution2.ParentExecution = execution3;

		entityManager.insert(execution2);
		entityManager.insert(execution3);

		// the parent (3) is inserted before the child (2)
		entityManager.flushEntityCache();
		IList<DbOperation> flush = entityManager.DbOperationManager.calculateFlush();
		assertHappensAfter(execution2, execution3, flush);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInsertReferenceOrderingAndIdOrdering()
	  public virtual void testInsertReferenceOrderingAndIdOrdering()
	  {

		execution2.ParentExecution = execution3;

		entityManager.insert(execution2);
		entityManager.insert(execution3);
		entityManager.insert(execution1);

		// the parent (3) is inserted before the child (2)
		entityManager.flushEntityCache();
		IList<DbOperation> flush = entityManager.DbOperationManager.calculateFlush();
		assertHappensAfter(execution2, execution3, flush);
		assertHappensAfter(execution3, execution1, flush);
		assertHappensAfter(execution2, execution1, flush);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInsertReferenceOrderingMultipleTrees()
	  public virtual void testInsertReferenceOrderingMultipleTrees()
	  {

		// tree1
		execution3.ParentExecution = execution4;
		execution2.ParentExecution = execution4;
		execution5.ParentExecution = execution3;

		// tree2
		execution1.ParentExecution = execution8;

		entityManager.insert(execution8);
		entityManager.insert(execution6);
		entityManager.insert(execution2);
		entityManager.insert(execution5);
		entityManager.insert(execution1);
		entityManager.insert(execution4);
		entityManager.insert(execution7);
		entityManager.insert(execution3);

		// the parent (3) is inserted before the child (2)
		entityManager.flushEntityCache();
		IList<DbOperation> insertOperations = entityManager.DbOperationManager.calculateFlush();
		assertHappensAfter(execution3, execution4, insertOperations);
		assertHappensAfter(execution2, execution4, insertOperations);
		assertHappensAfter(execution5, execution3, insertOperations);
		assertHappensAfter(execution1, execution8, insertOperations);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteReferenceOrdering()
	  public virtual void testDeleteReferenceOrdering()
	  {
		// given
		execution1.ParentExecution = execution2;
		entityManager.DbEntityCache.putPersistent(execution1);
		entityManager.DbEntityCache.putPersistent(execution2);

		// when deleting the entities
		entityManager.delete(execution1);
		entityManager.delete(execution2);

		entityManager.flushEntityCache();

		// then the flush is based on the persistent relationships
		IList<DbOperation> deleteOperations = entityManager.DbOperationManager.calculateFlush();
		assertHappensBefore(execution1, execution2, deleteOperations);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteReferenceOrderingAfterTransientUpdate()
	  public virtual void testDeleteReferenceOrderingAfterTransientUpdate()
	  {
		// given
		execution1.ParentExecution = execution2;
		entityManager.DbEntityCache.putPersistent(execution1);
		entityManager.DbEntityCache.putPersistent(execution2);

		// when reverting the relation in memory
		execution1.ParentExecution = null;
		execution2.ParentExecution = execution1;

		// and deleting the entities
		entityManager.delete(execution1);
		entityManager.delete(execution2);

		entityManager.flushEntityCache();

		// then the flush is based on the persistent relationships
		IList<DbOperation> deleteOperations = entityManager.DbOperationManager.calculateFlush();
		assertHappensBefore(execution1, execution2, deleteOperations);
	  }

	  protected internal virtual void assertHappensAfter(DbEntity entity1, DbEntity entity2, IList<DbOperation> operations)
	  {
		int idx1 = indexOfEntity(entity1, operations);
		int idx2 = indexOfEntity(entity2, operations);
		assertTrue("operation for " + entity1 + " should be executed after operation for " + entity2, idx1 > idx2);
	  }

	  protected internal virtual void assertHappensBefore(DbEntity entity1, DbEntity entity2, IList<DbOperation> operations)
	  {
		int idx1 = indexOfEntity(entity1, operations);
		int idx2 = indexOfEntity(entity2, operations);
		assertTrue("operation for " + entity1 + " should be executed before operation for " + entity2, idx1 < idx2);
	  }

	  protected internal virtual int indexOfEntity(DbEntity entity, IList<DbOperation> operations)
	  {
		for (int i = 0; i < operations.Count; i++)
		{
		  if (entity == ((DbEntityOperation) operations[i]).Entity)
		  {
			return i;
		  }
		}
		return -1;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInsertIdOrdering()
	  public virtual void testInsertIdOrdering()
	  {

		entityManager.insert(execution1);
		entityManager.insert(execution2);

		entityManager.flushEntityCache();
		IList<DbOperation> insertOperations = entityManager.DbOperationManager.calculateFlush();
		assertHappensAfter(execution2, execution1, insertOperations);
	  }

	  public class ExposingDbEntityManager : DbEntityManager
	  {

		public ExposingDbEntityManager(IdGenerator idGenerator, PersistenceSession persistenceSession) : base(idGenerator, persistenceSession)
		{
		}

		/// <summary>
		/// Expose this method for test purposes
		/// </summary>
		public override void flushEntityCache()
		{
		  base.flushEntityCache();
		}
	  }


	}

}