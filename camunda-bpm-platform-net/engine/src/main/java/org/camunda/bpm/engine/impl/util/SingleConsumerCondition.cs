using System;
using System.Threading;

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
namespace org.camunda.bpm.engine.impl.util
{

	/// <summary>
	/// MPSC Condition implementation.
	/// <para>
	/// Implementation Notes:
	/// <ul>
	/// <li><seealso cref="await(long)"/> may spuriously return before the deadline is reached.</li>
	/// <li>if <seealso cref="signal()"/> is called before the consumer thread calls <seealso cref="await(long)"/>,
	/// the next call to <seealso cref="await(long)"/> returns immediately.</li>
	/// </ul>
	/// </para>
	/// </summary>
	public class SingleConsumerCondition
	{

	  // note: making this private & final because it cannot be subclassed
	  // and replaced in a meaningful way without breaking the implementation
	  private readonly Thread consumer;

	  public SingleConsumerCondition(Thread consumer)
	  {
		if (consumer == null)
		{
		  throw new System.ArgumentException("Consumer thread cannot be null");
		}

		this.consumer = consumer;
	  }

	  public virtual void signal()
	  {
		LockSupport.unpark(consumer);
	  }

	  public virtual void await(long millis)
	  {
		if (Thread.CurrentThread != consumer)
		{
		  throw new Exception("Wrong usage of SingleConsumerCondition: can only await in consumer thread.");
		}

		// NOTE: may spuriously return before deadline
		LockSupport.parkNanos(TimeUnit.MILLISECONDS.toNanos(millis));
	  }

	}

}