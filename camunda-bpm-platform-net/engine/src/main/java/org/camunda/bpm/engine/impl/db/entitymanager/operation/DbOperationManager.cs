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
namespace org.camunda.bpm.engine.impl.db.entitymanager.operation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType.INSERT;


	using DbBulkOperationComparator = org.camunda.bpm.engine.impl.db.entitymanager.operation.comparator.DbBulkOperationComparator;
	using DbEntityOperationComparator = org.camunda.bpm.engine.impl.db.entitymanager.operation.comparator.DbEntityOperationComparator;
	using EntityTypeComparatorForInserts = org.camunda.bpm.engine.impl.db.entitymanager.operation.comparator.EntityTypeComparatorForInserts;
	using EntityTypeComparatorForModifications = org.camunda.bpm.engine.impl.db.entitymanager.operation.comparator.EntityTypeComparatorForModifications;

	/// <summary>
	/// Manages a set of <seealso cref="DbOperation database operations"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbOperationManager
	{

	  // comparators ////////////////

	  public static IComparer<Type> INSERT_TYPE_COMPARATOR = new EntityTypeComparatorForInserts();
	  public static IComparer<Type> MODIFICATION_TYPE_COMPARATOR = new EntityTypeComparatorForModifications();
	  public static IComparer<DbEntityOperation> INSERT_OPERATION_COMPARATOR = new DbEntityOperationComparator();
	  public static IComparer<DbEntityOperation> MODIFICATION_OPERATION_COMPARATOR = new DbEntityOperationComparator();
	  public static IComparer<DbBulkOperation> BULK_OPERATION_COMPARATOR = new DbBulkOperationComparator();

	  // pre-sorted operation maps //////////////

	  /// <summary>
	  /// INSERTs </summary>
	  public SortedDictionary<Type, SortedSet<DbEntityOperation>> inserts = new SortedDictionary<Type, SortedSet<DbEntityOperation>>(INSERT_TYPE_COMPARATOR);

	  /// <summary>
	  /// UPDATEs of a single entity </summary>
	  public SortedDictionary<Type, SortedSet<DbEntityOperation>> updates = new SortedDictionary<Type, SortedSet<DbEntityOperation>>(MODIFICATION_TYPE_COMPARATOR);

	  /// <summary>
	  /// DELETEs of a single entity </summary>
	  public SortedDictionary<Type, SortedSet<DbEntityOperation>> deletes = new SortedDictionary<Type, SortedSet<DbEntityOperation>>(MODIFICATION_TYPE_COMPARATOR);

	  /// <summary>
	  /// bulk modifications (DELETE, UPDATE) on an entity collection </summary>
	  public SortedDictionary<Type, SortedSet<DbBulkOperation>> bulkOperations = new SortedDictionary<Type, SortedSet<DbBulkOperation>>(MODIFICATION_TYPE_COMPARATOR);

	  /// <summary>
	  /// bulk modifications (DELETE, UPDATE) for which order of execution is important </summary>
	  public LinkedHashSet<DbBulkOperation> bulkOperationsInsertionOrder = new LinkedHashSet<DbBulkOperation>();

	  public virtual bool addOperation(DbEntityOperation newOperation)
	  {
		if (newOperation.OperationType == INSERT)
		{
		  return getInsertsForType(newOperation.EntityType, true).Add(newOperation);

		}
		else if (newOperation.OperationType == DELETE)
		{
		  return getDeletesByType(newOperation.EntityType, true).Add(newOperation);

		}
		else
		{ // UPDATE
		  return getUpdatesByType(newOperation.EntityType, true).Add(newOperation);

		}
	  }

	  protected internal virtual SortedSet<DbEntityOperation> getDeletesByType(Type type, bool create)
	  {
		SortedSet<DbEntityOperation> deletesByType = deletes[type];
		if (deletesByType == null && create)
		{
		  deletesByType = new SortedSet<DbEntityOperation>(MODIFICATION_OPERATION_COMPARATOR);
		  deletes[type] = deletesByType;
		}
		return deletesByType;
	  }

	  protected internal virtual SortedSet<DbEntityOperation> getUpdatesByType(Type type, bool create)
	  {
		SortedSet<DbEntityOperation> updatesByType = updates[type];
		if (updatesByType == null && create)
		{
		  updatesByType = new SortedSet<DbEntityOperation>(MODIFICATION_OPERATION_COMPARATOR);
		  updates[type] = updatesByType;
		}
		return updatesByType;
	  }

	  protected internal virtual SortedSet<DbEntityOperation> getInsertsForType(Type type, bool create)
	  {
		SortedSet<DbEntityOperation> insertsByType = inserts[type];
		if (insertsByType == null && create)
		{
		  insertsByType = new SortedSet<DbEntityOperation>(INSERT_OPERATION_COMPARATOR);
		  inserts[type] = insertsByType;
		}
		return insertsByType;
	  }

	  public virtual bool addOperation(DbBulkOperation newOperation)
	  {
		SortedSet<DbBulkOperation> bulksByType = bulkOperations[newOperation.EntityType];
		if (bulksByType == null)
		{
		  bulksByType = new SortedSet<DbBulkOperation>(BULK_OPERATION_COMPARATOR);
		  bulkOperations[newOperation.EntityType] = bulksByType;
		}

		return bulksByType.Add(newOperation);
	  }

	  public virtual bool addOperationPreserveOrder(DbBulkOperation newOperation)
	  {
		return bulkOperationsInsertionOrder.add(newOperation);
	  }

	  public virtual IList<DbOperation> calculateFlush()
	  {
		IList<DbOperation> flush = new List<DbOperation>();
		// first INSERTs
		addSortedInserts(flush);
		// then UPDATEs + DELETEs
		addSortedModifications(flush);
		return flush;
	  }

	  /// <summary>
	  /// Adds the insert operations to the flush (in correct order). </summary>
	  /// <param name="operationsForFlush">  </param>
	  protected internal virtual void addSortedInserts(IList<DbOperation> flush)
	  {
		foreach (KeyValuePair<Type, SortedSet<DbEntityOperation>> operationsForType in inserts.SetOfKeyValuePairs())
		{

		  // add inserts to flush
		  if (operationsForType.Key.IsAssignableFrom(typeof(HasDbReferences)))
		  {
			// if this type has self references, we need to resolve the reference order
			((IList<DbOperation>)flush).AddRange(sortByReferences(operationsForType.Value));
		  }
		  else
		  {
			((IList<DbOperation>)flush).AddRange(operationsForType.Value);
		  }
		}
	  }

	  /// <summary>
	  /// Adds a correctly ordered list of UPDATE and DELETE operations to the flush. </summary>
	  /// <param name="flush">  </param>
	  protected internal virtual void addSortedModifications(IList<DbOperation> flush)
	  {

		// calculate sorted set of all modified entity types
		SortedSet<Type> modifiedEntityTypes = new SortedSet<Type>(MODIFICATION_TYPE_COMPARATOR);
		modifiedEntityTypes.addAll(updates.Keys);
		modifiedEntityTypes.addAll(deletes.Keys);
		modifiedEntityTypes.addAll(bulkOperations.Keys);

		foreach (Type type in modifiedEntityTypes)
		{
		  // first perform entity UPDATES
		  addSortedModificationsForType(type, updates[type], flush);
		  // next perform entity DELETES
		  addSortedModificationsForType(type, deletes[type], flush);
		  // last perform bulk operations
		  SortedSet<DbBulkOperation> bulkOperationsForType = bulkOperations[type];
		  if (bulkOperationsForType != null)
		  {
			((IList<DbOperation>)flush).AddRange(bulkOperationsForType);
		  }
		}

		//the very last perform bulk operations for which the order is important
		if (bulkOperationsInsertionOrder != null)
		{
		  ((IList<DbOperation>)flush).AddRange(bulkOperationsInsertionOrder);
		}
	  }

	  protected internal virtual void addSortedModificationsForType(Type type, SortedSet<DbEntityOperation> preSortedOperations, IList<DbOperation> flush)
	  {
		if (preSortedOperations != null)
		{
		  if (type.IsAssignableFrom(typeof(HasDbReferences)))
		  {
			// if this type has self references, we need to resolve the reference order
			((IList<DbOperation>)flush).AddRange(sortByReferences(preSortedOperations));
		  }
		  else
		  {
			((IList<DbOperation>)flush).AddRange(preSortedOperations);
		  }
		}
	  }


	  /// <summary>
	  /// Assumptions:
	  /// a) all operations in the set work on entities such that the entities implement <seealso cref="HasDbReferences"/>.
	  /// b) all operations in the set work on the same type (ie. all operations are INSERTs or DELETEs).
	  /// 
	  /// </summary>
	  protected internal virtual IList<DbEntityOperation> sortByReferences(SortedSet<DbEntityOperation> preSorted)
	  {
		// copy the pre-sorted set and apply final sorting to list
		IList<DbEntityOperation> opList = new List<DbEntityOperation>(preSorted);

		for (int i = 0; i < opList.Count; i++)
		{

		  DbEntityOperation currentOperation = opList[i];
		  DbEntity currentEntity = currentOperation.Entity;
		  ISet<string> currentReferences = currentOperation.FlushRelevantEntityReferences;

		  // check whether this operation must be placed after another operation
		  int moveTo = i;
		  for (int k = i + 1; k < opList.Count; k++)
		  {
			DbEntityOperation otherOperation = opList[k];
			DbEntity otherEntity = otherOperation.Entity;
			ISet<string> otherReferences = otherOperation.FlushRelevantEntityReferences;

			if (currentOperation.OperationType == INSERT)
			{


			  // if we reference the other entity, we need to be inserted after that entity
			  if (currentReferences != null && currentReferences.Contains(otherEntity.Id))
			  {
				moveTo = k;
				break; // we can only reference a single entity
			  }

			}
			else
			{ // UPDATE or DELETE

			  // if the other entity has a reference to us, we must be placed after the other entity
			  if (otherReferences != null && otherReferences.Contains(currentEntity.Id))
			  {
				moveTo = k;
				// cannot break, there may be another entity further to the right which also references us
			  }

			}
		  }

		  if (moveTo > i)
		  {
			opList.Remove(i);
			opList.Insert(moveTo, currentOperation);
			i--;
		  }
		}

		return opList;
	  }
	}

}