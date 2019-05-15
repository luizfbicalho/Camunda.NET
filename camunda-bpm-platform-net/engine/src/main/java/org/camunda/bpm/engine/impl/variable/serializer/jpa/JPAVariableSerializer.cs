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
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// Variable type capable of storing reference to JPA-entities. Only JPA-Entities which
	/// are configured by annotations are supported. Use of compound primary keys is not supported.
	/// 
	/// @author Frederik Heremans
	/// @author Daniel Meyer
	/// </summary>
	public class JPAVariableSerializer : AbstractTypedValueSerializer<ObjectValue>
	{

	  public const string NAME = "jpa";

	  private JPAEntityMappings mappings;

	  public JPAVariableSerializer() : base(ValueType.OBJECT)
	  {
		mappings = new JPAEntityMappings();
	  }

	  public override string Name
	  {
		  get
		  {
			return NAME;
		  }
	  }

	  protected internal override bool canWriteValue(TypedValue value)
	  {
		if (isDeserializedObjectValue(value) || value is UntypedValueImpl)
		{
		  return value.Value == null || mappings.isJPAEntity(value.Value);
		}
		else
		{
		  return false;
		}
	  }

	  protected internal virtual bool isDeserializedObjectValue(TypedValue value)
	  {
		return value is ObjectValue && ((ObjectValue) value).Deserialized;
	  }

	  public override ObjectValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.objectValue(untypedValue.Value, untypedValue.Transient).create();
	  }

	  public virtual void writeValue(ObjectValue objectValue, ValueFields valueFields)
	  {
		EntityManagerSession entityManagerSession = Context.CommandContext.getSession(typeof(EntityManagerSession));
		if (entityManagerSession == null)
		{
		  throw new ProcessEngineException("Cannot set JPA variable: " + typeof(EntityManagerSession) + " not configured");
		}
		else
		{
		  // Before we set the value we must flush all pending changes from the entitymanager
		  // If we don't do this, in some cases the primary key will not yet be set in the object
		  // which will cause exceptions down the road.
		  entityManagerSession.flush();
		}

		object value = objectValue.Value;
		if (value != null)
		{
		  string className = mappings.getJPAClassString(value);
		  string idString = mappings.getJPAIdString(value);
		  valueFields.TextValue = className;
		  valueFields.TextValue2 = idString;
		}
		else
		{
		  valueFields.TextValue = null;
		  valueFields.TextValue2 = null;
		}
	  }

	  public override ObjectValue readValue(ValueFields valueFields, bool deserializeObjectValue)
	  {
		if (!string.ReferenceEquals(valueFields.TextValue, null) && !string.ReferenceEquals(valueFields.TextValue2, null))
		{
		  object jpaEntity = mappings.getJPAEntity(valueFields.TextValue, valueFields.TextValue2);
		  return Variables.objectValue(jpaEntity).create();
		}
		return Variables.objectValue(null).create();
	  }

	}

}