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
namespace org.camunda.bpm.engine.test.mock
{

	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;

	public class MockElResolver : ELResolver
	{

	  public override Type getCommonPropertyType(ELContext context, object @base)
	  {
		return typeof(object);
	  }

	  public override IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext context, object @base)
	  {
		return null;
	  }

	  public override Type getType(ELContext context, object @base, object property)
	  {
		return null;
	  }

	  public override object getValue(ELContext context, object @base, object property)
	  {
		object bean = Mocks.get(property);
		if (bean != null)
		{
		  context.PropertyResolved = true;
		}
		return bean;
	  }

	  public override bool isReadOnly(ELContext context, object @base, object property)
	  {
		return false;
	  }

	  public override void setValue(ELContext context, object @base, object property, object value)
	  {
	  }

	}

}