using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

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
namespace org.camunda.bpm.engine.test.standalone.jpa
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using AbstractProcessEngineTestCase = org.camunda.bpm.engine.impl.test.AbstractProcessEngineTestCase;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using EntityManagerSession = org.camunda.bpm.engine.impl.variable.serializer.jpa.EntityManagerSession;
	using EntityManagerSessionFactory = org.camunda.bpm.engine.impl.variable.serializer.jpa.EntityManagerSessionFactory;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using JavaSerializable = org.camunda.bpm.engine.test.api.variables.JavaSerializable;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Assert = org.junit.Assert;
	using Ignore = org.junit.Ignore;
	using TestSetup = junit.extensions.TestSetup;
	using Test = junit.framework.Test;
	using TestSuite = junit.framework.TestSuite;


	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class JPAVariableTest : AbstractProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/api/variables/oneTaskProcess.bpmn20.xml";
	  protected internal static ProcessEngine cachedProcessEngine;

	  private FieldAccessJPAEntity simpleEntityFieldAccess;
	  private PropertyAccessJPAEntity simpleEntityPropertyAccess;
	  private SubclassFieldAccessJPAEntity subclassFieldAccess;
	  private SubclassPropertyAccessJPAEntity subclassPropertyAccess;

	  private ByteIdJPAEntity byteIdJPAEntity;
	  private ShortIdJPAEntity shortIdJPAEntity;
	  private IntegerIdJPAEntity integerIdJPAEntity;
	  private LongIdJPAEntity longIdJPAEntity;
	  private FloatIdJPAEntity floatIdJPAEntity;
	  private DoubleIdJPAEntity doubleIdJPAEntity;
	  private CharIdJPAEntity charIdJPAEntity;
	  private StringIdJPAEntity stringIdJPAEntity;
	  private DateIdJPAEntity dateIdJPAEntity;
	  private SQLDateIdJPAEntity sqlDateIdJPAEntity;
	  private BigDecimalIdJPAEntity bigDecimalIdJPAEntity;
	  private BigIntegerIdJPAEntity bigIntegerIdJPAEntity;
	  private CompoundIdJPAEntity compoundIdJPAEntity;

	  private FieldAccessJPAEntity entityToQuery;
	  private FieldAccessJPAEntity entityToUpdate;

	  private static EntityManagerFactory entityManagerFactory;

	  // JUnit3-style beforeclass and afterclass
	  public static Test suite()
	  {
		return new JPAVariableTestSetup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore public static class JPAVariableTestSetup extends junit.extensions.TestSetup
	  public class JPAVariableTestSetup : TestSetup
	  {
		public JPAVariableTestSetup() : base(new TestSuite(typeof(JPAVariableTest)))
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
		protected internal virtual void setUp()
		{
		  ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/standalone/jpa/camunda.cfg.xml");

		  processEngineConfiguration.JavaSerializationFormatEnabled = true;

		  cachedProcessEngine = processEngineConfiguration.buildProcessEngine();

		  EntityManagerSessionFactory entityManagerSessionFactory = (EntityManagerSessionFactory) processEngineConfiguration.SessionFactories[typeof(EntityManagerSession)];

		  entityManagerFactory = entityManagerSessionFactory.EntityManagerFactory;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
		protected internal virtual void tearDown()
		{
		  cachedProcessEngine.close();
		  cachedProcessEngine = null;

		  entityManagerFactory.close();
		  entityManagerFactory = null;
		}

	  }

	  protected internal override void initializeProcessEngine()
	  {
		processEngine = cachedProcessEngine;
	  }

	  public virtual void setupJPAEntities()
	  {

		EntityManager manager = entityManagerFactory.createEntityManager();
		manager.Transaction.begin();

		// Simple test data
		simpleEntityFieldAccess = new FieldAccessJPAEntity();
		simpleEntityFieldAccess.Id = 1L;
		simpleEntityFieldAccess.Value = "value1";
		manager.persist(simpleEntityFieldAccess);

		simpleEntityPropertyAccess = new PropertyAccessJPAEntity();
		simpleEntityPropertyAccess.Id = 1L;
		simpleEntityPropertyAccess.Value = "value2";
		manager.persist(simpleEntityPropertyAccess);

		subclassFieldAccess = new SubclassFieldAccessJPAEntity();
		subclassFieldAccess.Id = 1L;
		subclassFieldAccess.Value = "value3";
		manager.persist(subclassFieldAccess);

		subclassPropertyAccess = new SubclassPropertyAccessJPAEntity();
		subclassPropertyAccess.Id = 1L;
		subclassPropertyAccess.Value = "value4";
		manager.persist(subclassPropertyAccess);

		// Test entities with all possible ID types
		byteIdJPAEntity = new ByteIdJPAEntity();
		byteIdJPAEntity.ByteId = (sbyte)1;
		manager.persist(byteIdJPAEntity);

		shortIdJPAEntity = new ShortIdJPAEntity();
		shortIdJPAEntity.ShortId = (short)123;
		manager.persist(shortIdJPAEntity);

		integerIdJPAEntity = new IntegerIdJPAEntity();
		integerIdJPAEntity.IntId = 123;
		manager.persist(integerIdJPAEntity);

		longIdJPAEntity = new LongIdJPAEntity();
		longIdJPAEntity.LongId = 123456789L;
		manager.persist(longIdJPAEntity);

		floatIdJPAEntity = new FloatIdJPAEntity();
		floatIdJPAEntity.FloatId = (float) 123.45678;
		manager.persist(floatIdJPAEntity);

		doubleIdJPAEntity = new DoubleIdJPAEntity();
		doubleIdJPAEntity.DoubleId = 12345678.987654;
		manager.persist(doubleIdJPAEntity);

		charIdJPAEntity = new CharIdJPAEntity();
		charIdJPAEntity.CharId = 'g';
		manager.persist(charIdJPAEntity);

		dateIdJPAEntity = new DateIdJPAEntity();
		dateIdJPAEntity.DateId = DateTime.Now;
		manager.persist(dateIdJPAEntity);

		sqlDateIdJPAEntity = new SQLDateIdJPAEntity();
		sqlDateIdJPAEntity.DateId = new java.sql.Date(new DateTime().Ticks);
		manager.persist(sqlDateIdJPAEntity);

		stringIdJPAEntity = new StringIdJPAEntity();
		stringIdJPAEntity.StringId = "azertyuiop";
		manager.persist(stringIdJPAEntity);

		bigDecimalIdJPAEntity = new BigDecimalIdJPAEntity();
		bigDecimalIdJPAEntity.BigDecimalId = new decimal("12345678912345678900000.123456789123456789");
		manager.persist(bigDecimalIdJPAEntity);

		bigIntegerIdJPAEntity = new BigIntegerIdJPAEntity();
		bigIntegerIdJPAEntity.BigIntegerId = BigInteger.Parse("12345678912345678912345678900000");
		manager.persist(bigIntegerIdJPAEntity);

		manager.flush();
		manager.Transaction.commit();
		manager.close();
	  }

	  public virtual void setupIllegalJPAEntities()
	  {
		EntityManager manager = entityManagerFactory.createEntityManager();
		manager.Transaction.begin();

		compoundIdJPAEntity = new CompoundIdJPAEntity();
		EmbeddableCompoundId id = new EmbeddableCompoundId();
		id.IdPart1 = 123L;
		id.IdPart2 = "part2";
		compoundIdJPAEntity.Id = id;
		manager.persist(compoundIdJPAEntity);

		manager.flush();
		manager.Transaction.commit();
		manager.close();
	  }

	  public virtual void setupQueryJPAEntity(long id)
	  {
		if (entityToQuery == null)
		{
		  EntityManager manager = entityManagerFactory.createEntityManager();
		  manager.Transaction.begin();

		  entityToQuery = new FieldAccessJPAEntity();
		  entityToQuery.Id = id;
		  manager.persist(entityToQuery);

		  manager.flush();
		  manager.Transaction.commit();
		  manager.close();
		}
	  }

	  public virtual void setupJPAEntityToUpdate()
	  {
		EntityManager manager = entityManagerFactory.createEntityManager();
		manager.Transaction.begin();

		entityToUpdate = new FieldAccessJPAEntity();
		entityToUpdate.Id = 3L;
		manager.persist(entityToUpdate);
		manager.flush();
		manager.Transaction.commit();
		manager.close();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testStoreJPAEntityAsVariable()
	  public virtual void testStoreJPAEntityAsVariable()
	  {
		setupJPAEntities();
		// -----------------------------------------------------------------------------
		// Simple test, Start process with JPA entities as variables
		// -----------------------------------------------------------------------------
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["simpleEntityFieldAccess"] = simpleEntityFieldAccess;
		variables["simpleEntityPropertyAccess"] = simpleEntityPropertyAccess;
		variables["subclassFieldAccess"] = subclassFieldAccess;
		variables["subclassPropertyAccess"] = subclassPropertyAccess;

		// Start the process with the JPA-entities as variables. They will be stored in the DB.
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("JPAVariableProcess", variables);

		// Read entity with @Id on field
		object fieldAccessResult = runtimeService.getVariable(processInstance.Id, "simpleEntityFieldAccess");
		Assert.assertTrue(fieldAccessResult is FieldAccessJPAEntity);
		Assert.assertEquals(1L, ((FieldAccessJPAEntity)fieldAccessResult).Id.Value);
		Assert.assertEquals("value1", ((FieldAccessJPAEntity)fieldAccessResult).Value);

		// Read entity with @Id on property
		object propertyAccessResult = runtimeService.getVariable(processInstance.Id, "simpleEntityPropertyAccess");
		Assert.assertTrue(propertyAccessResult is PropertyAccessJPAEntity);
		Assert.assertEquals(1L, ((PropertyAccessJPAEntity)propertyAccessResult).Id.Value);
		Assert.assertEquals("value2", ((PropertyAccessJPAEntity)propertyAccessResult).Value);

		// Read entity with @Id on field of mapped superclass
		object subclassFieldResult = runtimeService.getVariable(processInstance.Id, "subclassFieldAccess");
		Assert.assertTrue(subclassFieldResult is SubclassFieldAccessJPAEntity);
		Assert.assertEquals(1L, ((SubclassFieldAccessJPAEntity)subclassFieldResult).Id.Value);
		Assert.assertEquals("value3", ((SubclassFieldAccessJPAEntity)subclassFieldResult).Value);

		// Read entity with @Id on property of mapped superclass
		object subclassPropertyResult = runtimeService.getVariable(processInstance.Id, "subclassPropertyAccess");
		Assert.assertTrue(subclassPropertyResult is SubclassPropertyAccessJPAEntity);
		Assert.assertEquals(1L, ((SubclassPropertyAccessJPAEntity)subclassPropertyResult).Id.Value);
		Assert.assertEquals("value4", ((SubclassPropertyAccessJPAEntity)subclassPropertyResult).Value);

		// -----------------------------------------------------------------------------
		// Test updating JPA-entity to null-value and back again
		// -----------------------------------------------------------------------------
		object currentValue = runtimeService.getVariable(processInstance.Id, "simpleEntityFieldAccess");
		assertNotNull(currentValue);
		// Set to null
		runtimeService.setVariable(processInstance.Id, "simpleEntityFieldAccess", null);
		currentValue = runtimeService.getVariable(processInstance.Id, "simpleEntityFieldAccess");
		assertNull(currentValue);
		// Set to JPA-entity again
		runtimeService.setVariable(processInstance.Id, "simpleEntityFieldAccess", simpleEntityFieldAccess);
		currentValue = runtimeService.getVariable(processInstance.Id, "simpleEntityFieldAccess");
		assertNotNull(currentValue);
		Assert.assertTrue(currentValue is FieldAccessJPAEntity);
		Assert.assertEquals(1L, ((FieldAccessJPAEntity)currentValue).Id.Value);


		// -----------------------------------------------------------------------------
		// Test all allowed types of ID values
		// -----------------------------------------------------------------------------

		variables = new Dictionary<string, object>();
		variables["byteIdJPAEntity"] = byteIdJPAEntity;
		variables["shortIdJPAEntity"] = shortIdJPAEntity;
		variables["integerIdJPAEntity"] = integerIdJPAEntity;
		variables["longIdJPAEntity"] = longIdJPAEntity;
		variables["floatIdJPAEntity"] = floatIdJPAEntity;
		variables["doubleIdJPAEntity"] = doubleIdJPAEntity;
		variables["charIdJPAEntity"] = charIdJPAEntity;
		variables["stringIdJPAEntity"] = stringIdJPAEntity;
		variables["dateIdJPAEntity"] = dateIdJPAEntity;
		variables["sqlDateIdJPAEntity"] = sqlDateIdJPAEntity;
		variables["bigDecimalIdJPAEntity"] = bigDecimalIdJPAEntity;
		variables["bigIntegerIdJPAEntity"] = bigIntegerIdJPAEntity;

		// Start the process with the JPA-entities as variables. They will be stored in the DB.
		ProcessInstance processInstanceAllTypes = runtimeService.startProcessInstanceByKey("JPAVariableProcess", variables);
		object byteIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "byteIdJPAEntity");
		assertTrue(byteIdResult is ByteIdJPAEntity);
		assertEquals(byteIdJPAEntity.ByteId, ((ByteIdJPAEntity)byteIdResult).ByteId);

		object shortIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "shortIdJPAEntity");
		assertTrue(shortIdResult is ShortIdJPAEntity);
		assertEquals(shortIdJPAEntity.ShortId, ((ShortIdJPAEntity)shortIdResult).ShortId);

		object integerIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "integerIdJPAEntity");
		assertTrue(integerIdResult is IntegerIdJPAEntity);
		assertEquals(integerIdJPAEntity.IntId, ((IntegerIdJPAEntity)integerIdResult).IntId);

		object longIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "longIdJPAEntity");
		assertTrue(longIdResult is LongIdJPAEntity);
		assertEquals(longIdJPAEntity.LongId, ((LongIdJPAEntity)longIdResult).LongId);

		object floatIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "floatIdJPAEntity");
		assertTrue(floatIdResult is FloatIdJPAEntity);
		assertEquals(floatIdJPAEntity.FloatId, ((FloatIdJPAEntity)floatIdResult).FloatId);

		object doubleIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "doubleIdJPAEntity");
		assertTrue(doubleIdResult is DoubleIdJPAEntity);
		assertEquals(doubleIdJPAEntity.DoubleId, ((DoubleIdJPAEntity)doubleIdResult).DoubleId);

		object charIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "charIdJPAEntity");
		assertTrue(charIdResult is CharIdJPAEntity);
		assertEquals(charIdJPAEntity.CharId, ((CharIdJPAEntity)charIdResult).CharId);

		object stringIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "stringIdJPAEntity");
		assertTrue(stringIdResult is StringIdJPAEntity);
		assertEquals(stringIdJPAEntity.StringId, ((StringIdJPAEntity)stringIdResult).StringId);

		object dateIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "dateIdJPAEntity");
		assertTrue(dateIdResult is DateIdJPAEntity);
		assertEquals(dateIdJPAEntity.DateId, ((DateIdJPAEntity)dateIdResult).DateId);

		object sqlDateIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "sqlDateIdJPAEntity");
		assertTrue(sqlDateIdResult is SQLDateIdJPAEntity);
		assertEquals(sqlDateIdJPAEntity.DateId, ((SQLDateIdJPAEntity)sqlDateIdResult).DateId);

		object bigDecimalIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "bigDecimalIdJPAEntity");
		assertTrue(bigDecimalIdResult is BigDecimalIdJPAEntity);
		assertEquals(bigDecimalIdJPAEntity.BigDecimalId, ((BigDecimalIdJPAEntity)bigDecimalIdResult).BigDecimalId);

		object bigIntegerIdResult = runtimeService.getVariable(processInstanceAllTypes.Id, "bigIntegerIdJPAEntity");
		assertTrue(bigIntegerIdResult is BigIntegerIdJPAEntity);
		assertEquals(bigIntegerIdJPAEntity.BigIntegerId, ((BigIntegerIdJPAEntity)bigIntegerIdResult).BigIntegerId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIllegalEntities()
	  public virtual void testIllegalEntities()
	  {
		setupIllegalJPAEntities();
		// Starting process instance with a variable that has a compound primary key, which is not supported.
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["compoundIdJPAEntity"] = compoundIdJPAEntity;

		try
		{
		  runtimeService.startProcessInstanceByKey("JPAVariableProcessExceptions", variables);
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Cannot find field or method with annotation @Id on class", ae.Message);
		  assertTextPresent("only single-valued primary keys are supported on JPA-enities", ae.Message);
		}

		// Starting process instance with a variable that has null as ID-value
		variables = new Dictionary<string, object>();
		variables["nullValueEntity"] = new FieldAccessJPAEntity();

		try
		{
		  runtimeService.startProcessInstanceByKey("JPAVariableProcessExceptions", variables);
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Value of primary key for JPA-Entity is null", ae.Message);
		}

		// Starting process instance with an invalid type of ID
		// Under normal circumstances, JPA will throw an exception for this of the class is
		// present in the PU when creating EntityanagerFactory, but we test it *just in case*
		variables = new Dictionary<string, object>();
		IllegalIdClassJPAEntity illegalIdTypeEntity = new IllegalIdClassJPAEntity();
		illegalIdTypeEntity.Id = new DateTime();
		variables["illegalTypeId"] = illegalIdTypeEntity;

		try
		{
		  runtimeService.startProcessInstanceByKey("JPAVariableProcessExceptions", variables);
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("Unsupported Primary key type for JPA-Entity", ae.Message);
		}

		// Start process instance with JPA-entity which has an ID but isn't persisted. When reading
		// the variable we should get an exception.
		variables = new Dictionary<string, object>();
		FieldAccessJPAEntity nonPersistentEntity = new FieldAccessJPAEntity();
		nonPersistentEntity.Id = 9999L;
		variables["nonPersistentEntity"] = nonPersistentEntity;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("JPAVariableProcessExceptions", variables);

		try
		{
		  runtimeService.getVariable(processInstance.Id, "nonPersistentEntity");
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  assertTextPresent("Entity does not exist: " + typeof(FieldAccessJPAEntity).FullName + " - 9999", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testQueryJPAVariable()
	  public virtual void testQueryJPAVariable()
	  {
		setupQueryJPAEntity(2L);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["entityToQuery"] = entityToQuery;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("JPAVariableProcess", variables);

		// Query the processInstance
		ProcessInstance result = runtimeService.createProcessInstanceQuery().variableValueEquals("entityToQuery", entityToQuery).singleResult();
		assertNotNull(result);
		assertEquals(result.Id, processInstance.Id);

		// Query with the same entity-type but with different ID should have no result
		FieldAccessJPAEntity unexistingEntity = new FieldAccessJPAEntity();
		unexistingEntity.Id = 8888L;

		result = runtimeService.createProcessInstanceQuery().variableValueEquals("entityToQuery", unexistingEntity).singleResult();
		assertNull(result);

		// All other operators are unsupported
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueNotEquals("entityToQuery", entityToQuery).singleResult();
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueGreaterThan("entityToQuery", entityToQuery).singleResult();
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueGreaterThanOrEqual("entityToQuery", entityToQuery).singleResult();
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThan("entityToQuery", entityToQuery).singleResult();
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
		}
		try
		{
		  runtimeService.createProcessInstanceQuery().variableValueLessThanOrEqual("entityToQuery", entityToQuery).singleResult();
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("JPA entity variables can only be used in 'variableValueEquals'", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testUpdateJPAEntityValues()
	  public virtual void FAILING_testUpdateJPAEntityValues()
	  {
		setupJPAEntityToUpdate();
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["entityToUpdate"] = entityToUpdate;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("UpdateJPAValuesProcess", variables);

		// Servicetask in process 'UpdateJPAValuesProcess' should have set value on entityToUpdate.
		object updatedEntity = runtimeService.getVariable(processInstance.Id, "entityToUpdate");
		assertTrue(updatedEntity is FieldAccessJPAEntity);
		assertEquals("updatedValue", ((FieldAccessJPAEntity)updatedEntity).Value);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = ONE_TASK_PROCESS) public void testFailSerializationForUnknownSerializedValueType() throws java.io.IOException
	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testFailSerializationForUnknownSerializedValueType()
	  {
		// given
		JavaSerializable pojo = new JavaSerializable("foo");
		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(pojo);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), processEngine);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue serializedObjectValue = Variables.serializedObjectValue(serializedObject).serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName(pojo.GetType().FullName).create();
		VariableMap variables = Variables.createVariables().putValueTyped("var", serializedObjectValue);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// then
		JavaSerializable returnedPojo = (JavaSerializable) runtimeService.getVariable(processInstance.Id, "var");
		assertEquals(pojo, returnedPojo);
	  }

	}

}