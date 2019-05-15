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
namespace org.camunda.bpm.engine.impl.cfg
{

	using BeanFactory = org.springframework.beans.factory.BeanFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class SpringBeanFactoryProxyMap : IDictionary<object, object>
	{

	  protected internal BeanFactory beanFactory;

	  public SpringBeanFactoryProxyMap(BeanFactory beanFactory)
	  {
		this.beanFactory = beanFactory;
	  }

	  public virtual object get(object key)
	  {
		if ((key == null) || (!key.GetType().IsAssignableFrom(typeof(string))))
		{
		  return null;
		}
		return beanFactory.getBean((string) key);
	  }

	  public virtual bool ContainsKey(object key)
	  {
		if ((key == null) || (!key.GetType().IsAssignableFrom(typeof(string))))
		{
		  return false;
		}
		return beanFactory.containsBean((string) key);
	  }

	  public virtual ISet<object> keySet()
	  {
		return Collections.emptySet();
	  }

	  public virtual void Clear()
	  {
		throw new ProcessEngineException("can't clear configuration beans");
	  }

	  public virtual bool containsValue(object value)
	  {
		throw new ProcessEngineException("can't search values in configuration beans");
	  }

	  public virtual ISet<KeyValuePair<object, object>> entrySet()
	  {
		throw new ProcessEngineException("unsupported operation on configuration beans");
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			throw new ProcessEngineException("unsupported operation on configuration beans");
		  }
	  }

	  public virtual object put(object key, object value)
	  {
		throw new ProcessEngineException("unsupported operation on configuration beans");
	  }

	  public virtual void putAll<T1>(IDictionary<T1> m) where T1 : object
	  {
		throw new ProcessEngineException("unsupported operation on configuration beans");
	  }

	  public virtual object remove(object key)
	  {
		throw new ProcessEngineException("unsupported operation on configuration beans");
	  }

	  public virtual int Count
	  {
		  get
		  {
			throw new ProcessEngineException("unsupported operation on configuration beans");
		  }
	  }

	  public virtual ICollection<object> Values
	  {
		  get
		  {
			throw new ProcessEngineException("unsupported operation on configuration beans");
		  }
	  }
	}

}