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
namespace org.camunda.bpm.engine.impl.variable.serializer.jpa
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class JPAEntityMappings
	{

	  private IDictionary<string, EntityMetaData> classMetaDatamap;

	  private JPAEntityScanner enitityScanner;

	  public JPAEntityMappings()
	  {
		classMetaDatamap = new Dictionary<string, EntityMetaData>();
		enitityScanner = new JPAEntityScanner();
	  }

	  public virtual bool isJPAEntity(object value)
	  {
		if (value != null)
		{
		  // EntityMetaData will be added for all classes, even those who are not 
		  // JPA-entities to prevent unneeded annotation scanning  
		  return getEntityMetaData(value.GetType()).JPAEntity;
		}
		return false;
	  }

	  private EntityMetaData getEntityMetaData(Type clazz)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		EntityMetaData metaData = classMetaDatamap[clazz.FullName];
		if (metaData == null)
		{
		  // Class not present in meta-data map, create metaData for it and add
		  metaData = scanClass(clazz);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  classMetaDatamap[clazz.FullName] = metaData;
		}
		return metaData;
	  }

	  private EntityMetaData scanClass(Type clazz)
	  {
		return enitityScanner.scanClass(clazz);
	  }

	  public virtual string getJPAClassString(object value)
	  {
		ensureNotNull("null value cannot be saved", "value", value);

		EntityMetaData metaData = getEntityMetaData(value.GetType());
		if (!metaData.JPAEntity)
		{
		  throw new ProcessEngineException("Object is not a JPA Entity: class='" + value.GetType() + "', " + value);
		}

		// Extract the ID from the Entity instance using the metaData
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return metaData.EntityClass.FullName;
	  }

	  public virtual string getJPAIdString(object value)
	  {
		EntityMetaData metaData = getEntityMetaData(value.GetType());
		if (!metaData.JPAEntity)
		{
		  throw new ProcessEngineException("Object is not a JPA Entity: class='" + value.GetType() + "', " + value);
		}
		object idValue = getIdValue(value, metaData);
		return getIdString(idValue);
	  }

	  private object getIdValue(object value, EntityMetaData metaData)
	  {
		try
		{
		  if (metaData.IdMethod != null)
		  {
			return metaData.IdMethod.invoke(value);
		  }
		  else if (metaData.IdField != null)
		  {
			return metaData.IdField.get(value);
		  }
		}
		catch (System.ArgumentException iae)
		{
		  throw new ProcessEngineException("Illegal argument exception when getting value from id method/field on JPAEntity", iae);
		}
		catch (IllegalAccessException iae)
		{
		  throw new ProcessEngineException("Cannot access id method/field for JPA Entity", iae);
		}
		catch (InvocationTargetException ite)
		{
		  throw new ProcessEngineException("Exception occured while getting value from id field/method on JPAEntity: " + ite.InnerException.Message, ite.InnerException);
		}

		// Fall trough when no method and field is set
		throw new ProcessEngineException("Cannot get id from JPA Entity, no id method/field set");
	  }

	  public virtual object getJPAEntity(string className, string idString)
	  {
		Type entityClass = null;
		entityClass = ReflectUtil.loadClass(className);

		EntityMetaData metaData = getEntityMetaData(entityClass);
		ensureNotNull("Class is not a JPA-entity: " + className, "metaData", metaData);

		// Create primary key of right type
		object primaryKey = createId(metaData, idString);
		return findEntity(entityClass, primaryKey);
	  }

	  private object findEntity(Type entityClass, object primaryKey)
	  {
		EntityManager em = Context.CommandContext.getSession(typeof(EntityManagerSession)).EntityManager;

		object entity = em.find(entityClass, primaryKey);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ensureNotNull("Entity does not exist: " + entityClass.FullName + " - " + primaryKey, "entity", entity);
		return entity;
	  }

	  public virtual object createId(EntityMetaData metaData, string @string)
	  {
		Type type = metaData.IdType;
		// According to JPA-spec all primitive types (and wrappers) are supported, String, util.Date, sql.Date,
		// BigDecimal and BigInteger
		if (type == typeof(Long) || type == typeof(long))
		{
		  return long.Parse(@string);
		}
		else if (type == typeof(string))
		{
		  return @string;
		}
		else if (type == typeof(Byte) || type == typeof(sbyte))
		{
		  return sbyte.Parse(@string);
		}
		else if (type == typeof(Short) || type == typeof(short))
		{
		  return short.Parse(@string);
		}
		else if (type == typeof(Integer) || type == typeof(int))
		{
		  return int.Parse(@string);
		}
		else if (type == typeof(Float) || type == typeof(float))
		{
		  return float.Parse(@string);
		}
		else if (type == typeof(Double) || type == typeof(double))
		{
		  return double.Parse(@string);
		}
		else if (type == typeof(Character) || type == typeof(char))
		{
		  return new char?(@string[0]);
		}
		else if (type == typeof(DateTime))
		{
		  return new DateTime(long.Parse(@string));
		}
		else if (type == typeof(java.sql.Date))
		{
		  return new java.sql.Date(long.Parse(@string));
		}
		else if (type == typeof(decimal))
		{
		  return new decimal(@string);
		}
		else if (type == typeof(System.Numerics.BigInteger))
		{
		  return new System.Numerics.BigInteger(@string);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ProcessEngineException("Unsupported Primary key type for JPA-Entity: " + type.FullName);
		}
	  }

	  public virtual string getIdString(object value)
	  {
		ensureNotNull("Value of primary key for JPA-Entity", value);
		// Only java.sql.date and java.util.date require custom handling, the other types
		// can just use toString()
		if (value is DateTime)
		{
		  return "" + ((DateTime) value).Ticks;
		}
		else if (value is java.sql.Date)
		{
		  return "" + ((java.sql.Date) value).Time;
		}
		else if (value is long? || value is string || value is sbyte? || value is short? || value is int? || value is float? || value is double? || value is char? || value is decimal || value is System.Numerics.BigInteger)
		{
		  return value.ToString();
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ProcessEngineException("Unsupported Primary key type for JPA-Entity: " + value.GetType().FullName);
		}
	  }
	}

}