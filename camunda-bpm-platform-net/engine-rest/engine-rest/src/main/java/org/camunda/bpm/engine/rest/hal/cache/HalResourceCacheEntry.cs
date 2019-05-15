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
namespace org.camunda.bpm.engine.rest.hal.cache
{
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	public class HalResourceCacheEntry
	{

	  protected internal string id;
	  protected internal long created;
	  protected internal object resource;

	  public HalResourceCacheEntry(string id, object resource)
	  {
		this.id = id;
		this.created = ClockUtil.CurrentTime.Ticks;
		this.resource = resource;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual long CreateTime
	  {
		  get
		  {
			return created;
		  }
	  }

	  public virtual object Resource
	  {
		  get
		  {
			return resource;
		  }
	  }

	}

}